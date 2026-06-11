docker build -t learnup -f src\Learnup.API\Dockerfile .
docker tag learnup registry.itsaze.com/learnup
docker push registry.itsaze.com/learnup