using Learnup.Application.Responses.Public.Courses;
using Learnup.Domain.AggregateRoots.Courses;

namespace Learnup.Application.Mappers;

public static class CourseMapper
{
    public static CourseResponse ToResponse(
        this Course course) =>
        new(
            course.Id,
            course.Code,
            course.Slug,
            course.Title,
            course.Description,
            course.Order,
            course.Lessons.Count,
            course.Lessons
                .SelectMany(lesson => lesson.Users)
                .Count(userLesson => userLesson.CompletedAt != null),
            course.LanguageId,
            course.CoverId,
            course.Lessons
                .SelectMany(lesson => lesson.Users)
                .OrderByDescending(userLesson => userLesson.LastVisitedAt)
                .Select(userLesson => (DateTime?)userLesson.LastVisitedAt)
                .FirstOrDefault());
}
