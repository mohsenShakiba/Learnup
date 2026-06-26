docker build -t learnup -f src\Learnup.API\Dockerfile .
docker tag learnup 158.255.74.102:5000/learnup
docker push 158.255.74.102:5000/learnup