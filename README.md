# hm-doctor-service

Independent microservice repository for Hospital Management.

## Local run

`ash
dotnet restore
dotnet build
dotnet run --project src/DoctorService.Api/DoctorService.Api.csproj
`

## Docker

`ash
docker build -t hm-doctor-service:local .
docker run -p 8083:8080 hm-doctor-service:local
`

## GitHub setup later

`ash
git branch -M main
git remote add origin <your-github-repo-url>
git add .
git commit -m "Initial scaffold"
git push -u origin main
`
