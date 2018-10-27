# Accrual Calculator

## Monitor your paid time off accruals and usage over time
View me live at https://accrual-calculator-prod.herokuapp.com!

### Tech Stack
* macOS + Jetbrains Rider
* C# / .NET Core / ASP.NET Core 2.1
* MongoDB
* GraphQL + GraphQL Playground
* Auth0 (GitHub/Google/local account) for authentication

### Usage

#### Creating HTTPS Certificate
1. Instructions for reference: https://github.com/dotnet/dotnet-docker/blob/master/samples/aspnetapp/aspnetcore-docker-https-development.md#macos.
1. Open a command prompt to `/src/presentation/AccrualCalculator.Web`.
1. Generate cert and configure local machine:  
`dotnet dev-certs https -ep ${HOME}/.aspnet/https/AccrualCalculator.Web.pfx -p crypticpassword`
`dotnet dev-certs https --trust`

#### Manually
1. Clone the app.
1. Build Solution via Visual Studio 2017.
1. Play.
1. Hosted Services
    * Auth0
    * MongoDB Atlas
1. API Keys:
    * SendGrid
    * Recaptcha
    * GitHub
    * Google

#### Docker
1. Make a copy of the file `example.env` to `PRIVATE` and fill in the missing values with your API keys.
1. Open a command prompt to `/src/presentation/AccrualCalculator.Web`.
1. Run the command `docker-compose up --build` to build the image and start the containers.
1. Open your browser to `https://localhost:50443`.

### Current Limitations
* GraphQL route protection could use some improvement but for now it works.
* Code documentation needs added.