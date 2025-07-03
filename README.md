
# Carton Caps Referral Service

Referral Microservice handles generation of user referral code, creation of referrals, and completing the referrals once an invite deep link converts into a registration.


## Decisions and Assumptions

- Minimal API was used for performance and considering this to be a microservice project without the need of controller/views overhead.
- EF Core was not used to showcase lower implementation functionality and speed of hangfire. A hybrid approach could also be use with CQRS pattern.
- The notion of an account (user) was added since this would be a small part of a larger application with IDP management elsewhere. Instead of in the routes the account/userIds could also be pass view claims in the token. Accounts are not needed in a purely loyalty system as the referral code is the only unique identifier needed.
- Integrations test project was added over unit test to showcase a hollistic testing approach using the build in support for .net to create a fully in memory api.
- Sqlite was used for easier development and mocking but can easily be swapped for any other storage solution.
- Notes were left in key areas and the author is aware of alternative improvements and changes like layer structure clean/vertical slice but kept everything self contained. Other areas like referral code generation were implemented in naive way with the knowledge that it could be put behind a contract and/or easily swapped out.

  https://www.loom.com/share/0426437b0ea740d0b38a2c1338fb9b23
  
## Running

Project is completely selft contained but an appservice instance has been created to explore the API and FrontEnd developers.

https://referral-service-fee2dfe0avekdfhe.westus-01.azurewebsites.net/swagger/index.html

Sample Flow:

    1. /accounts - gets you a list of existing accounts and their referrals. 
    2. Use any accountId to create a referral /referrals POST method
    3. Call /referrals/{referralId}/complete to simulate conversion.




## Tech Stack

**Server:** .Net Core 9, xUnit, SqLite, Dapper, Minimal API, Swagger

