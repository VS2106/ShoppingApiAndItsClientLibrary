Please open the project by Visual Studio 2015 and newer. </br>
Before running the project, please restore Nuget packages. </br></br>

ShoppingApi client library is located at ShoppingAPI/Client/ShoppingApiClient.js </br>
A demo demonstrating how to use this library is located at ShoppingAPI/Client/Demo.html </br></br>

ShoppingAPI.Tests is a normal unit test project.</br>
ShoppingAPI.IntegrationTests is an integration test project with a real persistence layer.</br>
Tests use Nunit 3, NUnit3TestAdapter liberary is included in the project, you could only see all tests in test explorer after first build.(unless you installed VS extension NUnit 3 Test Adapter)</br></br>

This project applies ef code first approach. it will generate and update database according to migrations.</br>
For the demo project, I seed the database with fake users and products. 
(migration file ShoppingAPI/Persistence/Migrations/201802162006577_SeedDBWithFakeTestData.cs)</br></br>

ShoppingApi has 3 business domain models.</br>
*ShoppingBasket (each registered user must have one, and only one, ShoppingBasket)</br>
*OrderItem, a line of an order.(a ShoppingBasket has a collection of OrderItems, but a ShoppingBasket cannot have multiple OrderItems with the same product)</br>
*Product</br></br>

by the domian model mapping, ShoppingApi is aware of data concurrency issue and unique index issue (see ShoppingAPI.IntegrationTests/Persistence/)</br></br>

ShoppingApi applies repository pattern, unit of work pattern, dependency injection pattern ...</br></br>

If you want to enable SSL for the web api, check out branch masterEnableSSL. </br>
To continue, please choose to trust the self-signed certificated that IIS Express has generated. 
