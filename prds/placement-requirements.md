# Requirements: Placement Test

Engineering requirements derived from [placement.md](placement.md). Targets the existing
.NET 10 clean-architecture solution (`Domain` / `Application` / `Infrastructure` / `API`) and
follows current conventions: vertical-slice features, custom mediator (`IRequest` / `IRequestHandler`),
`sealed record` requests, `internal sealed` handlers, `ILearnupDbContext`, `Infrastructure/Loaders`
importers, and area-based controllers.

> **`placement_test.json` is imported into the database**, not served from disk at runtime. A
> `PlacementTest` aggregate is seeded via an admin import endpoint (mirroring the existing
> Story/Grammar/Vocab import flow). Runtime read/submit endpoints then work off the DB rows.

> **Relationship to the existing `Test` aggregate:** the current `Test` / `TestOption` /
> `UserTestResult` types are *per-lesson* comprehension checks. The placement test is a separate
> concept (standalone CEFR routing quiz) and gets its own aggregate + namespace.

---

## 1. Scope

- **Import** the 24-question CEFR placement quiz from `placement_test.json` into the DB (admin).
- **Serve** the test to learners (questions + options, **never** the correct answer).
- **Grade** a submitted answer set server-side → placed CEFR level (A1–C2).
- **Persist** the learner's result (upsert, one row per user) for lesson routing.
- **Route** the placed level to the matching `Course` via its `Code` (already `A1`–`C2`).

Out of scope (PRD non-goals): adaptive/branching logic, writing/speaking/listening items,
skipping already-mastered lessons.

---

## 2. Domain (`Learnup.Domain`)

### 2.1 New enum: `CefrLevel`

`Learnup.Domain.AggregateRoots.Placement` (or a shared location):

```csharp
public enum CefrLevel { A1 = 1, A2 = 2, B1 = 3, B2 = 4, C1 = 5, C2 = 6 }
```

Dedicated CEFR enum rather than reusing `VocabLevel` (which carries `Unknown`/`Common` members
that are meaningless for a course band or a placement result).

### 2.2 New enum: `PlacementSkill`

```csharp
public enum PlacementSkill { Grammar = 1, Vocabulary = 2 }
```

### 2.3 New aggregate: `PlacementTest`

Namespace `Learnup.Domain.AggregateRoots.Placement`. Encapsulated setters + private ctor, matching
`Test`/`Story` style.

**`PlacementTest`** (root)

| Field          | Type                            | Notes                          |
|----------------|---------------------------------|--------------------------------|
| `Id`           | `int`                           | PK                             |
| `Title`        | `string`                        |                                |
| `Description`  | `string`                        | Persian                        |
| `Instructions` | `string`                        | Persian                        |
| `Questions`    | `IReadOnlyList<PlacementQuestion>` | backed by `_questions`      |

**`PlacementQuestion`**

| Field             | Type                             | Notes                                            |
|-------------------|----------------------------------|--------------------------------------------------|
| `Id`              | `int`                            | PK (DB identity)                                 |
| `PlacementTestId` | `int`                            | FK                                               |
| `Number`          | `int`                            | The JSON `id` (1–24); question order             |
| `Level`           | `CefrLevel`                      | Band                                             |
| `Skill`           | `PlacementSkill`                 |                                                  |
| `Prompt`          | `string`                         |                                                  |
| `Options`         | `IReadOnlyList<PlacementOption>` | backed by `_options`                             |

**`PlacementOption`**

| Field                 | Type     | Notes                              |
|-----------------------|----------|------------------------------------|
| `Id`                  | `int`    | PK                                 |
| `PlacementQuestionId` | `int`    | FK                                 |
| `Text`                | `string` |                                    |
| `IsCorrect`           | `bool`   | Exactly one `true` per question    |

- Grading uses `IsCorrect` (the JSON `answer` is resolved to the matching option's `IsCorrect`
  flag at import time). The correct answer is **never** projected into the public read model.

### 2.4 New entity: `UserPlacementResult`

Namespace `Learnup.Domain.AggregateRoots.Users` (alongside `UserTestResult`).

| Field           | Type         | Notes                                            |
|-----------------|--------------|--------------------------------------------------|
| `Id`            | `int`        | PK                                               |
| `UserId`        | `int`        | FK → `User` (unique — one row per user, upsert)  |
| `PlacedLevel`   | `CefrLevel`  | Result band                                      |
| `CorrectByBand` | `string`     | JSON snapshot of per-band correct counts (audit) |
| `TakenAt`       | `DateTime`   | UTC                                              |

- Private parameterless ctor + public ctor `(userId, placedLevel, correctByBand)` setting
  `TakenAt = DateTime.UtcNow`; an `Update(placedLevel, correctByBand)` method for the upsert path
  (mirrors `UserTestResult.UpdateSelectedOption`).

### 2.5 `Course` — no schema change

`Course.Code` is already the CEFR band (`A1`–`C2`), so routing (§7) matches on `Code` directly.
**No new column** is added — a separate `CefrLevel` property would duplicate `Code` and require
keeping the two in sync.

---

## 3. Infrastructure (`Learnup.Infrastructure`)

- **`ILearnupDbContext`** + `LearnupDbContext`: add
  `DbSet<PlacementTest>`, `DbSet<PlacementQuestion>`, `DbSet<PlacementOption>`,
  `DbSet<UserPlacementResult>`.
- **EF configurations** in `Infrastructure/Configurations`:
  - `PlacementTestConfiguration`, `PlacementQuestionConfiguration`, `PlacementOptionConfiguration`
    (owned collections / relationships, `CefrLevel` & `PlacementSkill` stored as `int`).
  - `UserPlacementResultConfiguration` — **unique index on `UserId`** (enforces one-per-user).
- **Migration:** `AddPlacementTest` (creates placement tables + `UserPlacementResult`).
  No `Course` change.

### 3.1 Importer: `PlacementTestLoader`

`Infrastructure/Loaders/PlacementTestLoader.cs` (mirrors `StoryLoader`, ctor-injects
`LearnupDbContext`):

```csharp
public async Task<int> LoadAsync(PlacementTestRequest request, CancellationToken ct = default)
```

Behavior:
1. **Validate** the payload (§3.2). Throw `InvalidOperationException` / `FormatException` on failure.
2. In a transaction: **replace** any existing placement test (delete old test + cascade), insert the
   new `PlacementTest` with its questions/options. (v1 has a single active placement test — reimport
   overwrites.)
3. For each question, resolve `answer` → set `IsCorrect = true` on the exactly-one matching option.
4. `SaveChanges`, commit, return the new `PlacementTest.Id`.

### 3.2 Import validation (enforced in the loader)

- Exactly **24** questions; exactly **4** per band for A1, A2, B1, B2, C1, C2.
- Questions ordered ascending by band (A1→C2) then by `id`; `id` values 1–24 unique.
- Each question: exactly **4** distinct options; `answer` matches **exactly one** option.
- Every `level` parses to `CefrLevel`; every `skill` parses to `PlacementSkill`.

---

## 4. Import Contract & Endpoint

### 4.1 Request DTO (`Learnup.Application/Requests/Admin/Placement/`)

Mirrors the JSON schema in the PRD:

```csharp
public record PlacementTestRequest(
    string Title,
    string Description,
    string Instructions,
    List<PlacementQuestionRequest> Questions);

public record PlacementQuestionRequest(
    int Id,
    string Level,        // "A1".."C2"
    string Skill,        // "grammar" | "vocabulary"
    string Prompt,
    List<string> Options,
    string Answer);
```

(The `scoring` block from the JSON is descriptive/human-facing and is **not** imported — the rule is
implemented in code, §6. Deserializer ignores unmapped fields.)

### 4.2 Endpoint — `ImportController` (`Areas/Admin`)

Add to [ImportController.cs](src/Learnup.API/Areas/Admin/Controllers/ImportController.cs) (inject
`PlacementTestLoader`):

```
POST admin/import/placement-test
Body: PlacementTestRequest (application/json)
→ 200 OK  { placementTestId }
→ 400 BadRequest  (validation failure message)
```

Follows the existing `ImportStory` / `ImportGrammar` JSON-body pattern (thin controller,
try/catch → `BadRequest` on `InvalidOperationException`/`FormatException`).

---

## 5. Application Layer — runtime slices

Folder `Features/Public/Placement/`; responses in `Responses/Public/Placement/`.

### 5.1 `GetPlacementTest` (query)

- `sealed record GetPlacementTest : IRequest<PlacementTestResponse?>`
- Handler reads the single `PlacementTest` (with questions + options ordered by `Number`), maps to
  response **without `IsCorrect`/answer**.

```csharp
sealed record PlacementTestResponse(
    int Id, string Title, string Description, string Instructions,
    IReadOnlyList<PlacementQuestionResponse> Questions);

sealed record PlacementQuestionResponse(
    int Id, int Number, CefrLevel Level, PlacementSkill Skill, string Prompt,
    IReadOnlyList<PlacementOptionResponse> Options);

sealed record PlacementOptionResponse(int Id, string Text);   // NO IsCorrect
```

### 5.2 `SubmitPlacementTest` (command)

```csharp
sealed record SubmitPlacementTest(IReadOnlyList<PlacementSubmissionAnswer> Answers)
    : IRequest<PlacementResultResponse>;
sealed record PlacementSubmissionAnswer(int QuestionId, int SelectedOptionId);
```

Handler (`internal sealed`, injects `ILearnupDbContext`, `IIdentityProvider`):
1. Load the placement test's questions + options.
2. Grade each answer via `PlacementOption.IsCorrect` (missing/invalid answer = incorrect).
3. Compute `correctByBand` (CefrLevel → count) and apply the scoring rule (§6) → `placedLevel`.
4. **Upsert** `UserPlacementResult` for `identityProvider.UserId` (`Update` if exists, else insert);
   `SaveChangesAsync`.
5. Return result.

```csharp
sealed record PlacementResultResponse(
    CefrLevel PlacedLevel,
    IReadOnlyDictionary<CefrLevel, int> CorrectByBand,
    int? StartingCourseId);   // resolved via §7, null if untagged
```

### 5.3 `GetPlacementResult` (query)

- Returns the caller's stored `UserPlacementResult` (or `null` if not taken) so the client can skip
  straight to lessons.

### 5.4 Scorer

`PlacementScorer` — pure static function `CefrLevel Score(IReadOnlyDictionary<CefrLevel,int> correctByBand)`,
unit-tested independently of EF (see §6).

---

## 6. Scoring Rule (deterministic)

- A band is **passed** when `correct >= 3` of its 4 questions.
- Placed level = the **highest** band such that it **and every lower band** are passed.
- No band passed → **A1**.

```
bands = [A1, A2, B1, B2, C1, C2]      // ascending
placed = A1
foreach band in bands:
    if passed(band): placed = band
    else:            break             // stop at first gap
return placed
```

Total order over bands → single deterministic output for any answer set.

---

## 7. Routing to a Starting Course

`IStartingCourseResolver` (or inline in the submit handler): map `placedLevel` → the `Course` whose
`Code == placedLevel.ToString()` (e.g. `C1`), return its `Id`. If no matching course exists,
`StartingCourseId` is `null` and the client falls back to the course list.

Uses the existing `Course.Code` (already `A1`–`C2`) — no schema change, no data tagging needed.

---

## 8. API — Public endpoints

`PlacementController : BasePublicController` (`Areas/Public`, `[Authorize]`, route `[area]/[controller]`).

| Method & Route                  | Action    | Body                       | Response                    |
|---------------------------------|-----------|----------------------------|-----------------------------|
| `GET  mobile/placement`         | GetTest   | —                          | `PlacementTestResponse` / 404 |
| `POST mobile/placement/submit`  | Submit    | `SubmitPlacementRequest`   | `PlacementResultResponse`   |
| `GET  mobile/placement/result`  | GetResult | —                          | `PlacementResultResponse?` / 204 |

API request DTO in `src/Learnup.API/Requests/`:

```csharp
public sealed record SubmitPlacementRequest(IReadOnlyList<PlacementAnswerDto> Answers);
public sealed record PlacementAnswerDto(int QuestionId, int SelectedOptionId);
```

Thin controllers: build mediator request → `Send` → `Ok`/`NotFound`.

---

## 9. Acceptance Criteria (traceable to PRD)

- [ ] `POST admin/import/placement-test` validates the payload and persists a `PlacementTest`:
      24 questions, 4/band, ascending, each `answer` matches exactly one option (→ `IsCorrect`).
- [ ] Reimport overwrites the previous placement test (single active test).
- [ ] `GET mobile/placement` returns all 24 questions + options with **no correct-answer flag**.
- [ ] `POST mobile/placement/submit` grades server-side and returns a single deterministic level
      for any answer set (§6). Unit tests: all-correct → C2; none → A1; pass A1/A2 fail B1 → A2;
      pass A1, fail A2 but pass B1 → A1 (gap rule).
- [ ] Result upserts one `UserPlacementResult` per user; readable via `GET mobile/placement/result`.
- [ ] Placed level maps to a `Course` via `Code` (or `null` fallback).
- [ ] Public endpoints require auth; import endpoint is under the Admin area.

---

## 10. Work Breakdown

1. **Domain:** `CefrLevel`, `PlacementSkill` enums; `PlacementTest`/`PlacementQuestion`/`PlacementOption`
   aggregate; `UserPlacementResult`.
2. **Infrastructure:** `DbSet`s, EF configs (+ unique index on `UserPlacementResult.UserId`),
   migration `AddPlacementTest`; `PlacementTestLoader` + validation.
3. **Import:** `PlacementTestRequest` DTOs; `ImportController` endpoint.
4. **Application:** `GetPlacementTest`, `SubmitPlacementTest`, `GetPlacementResult` slices +
   responses; `PlacementScorer` pure function.
5. **API:** `PlacementController`, `SubmitPlacementRequest` DTO.
6. **Routing:** `IStartingCourseResolver` over `Course.Code`.
7. **Tests:** loader validation, scorer edge cases (§9), submit-handler grading + upsert.

---

## 11. Open Questions

_All resolved:_

- **Content** — `placement_test.json` authored (24 items, Persian UI strings), matches §4.1.
- **Course routing** — `Course.Code` is already `A1`–`C2`; resolver matches on it (§7), no data task.
- **Enum / retake / result naming** — dedicated `CefrLevel`; upsert one `UserPlacementResult`/user.
