# WebApiAppDB
You should create a Web API, that enables operations with the Northwind Database (script with DB structure and test data can be found here - https://github.com/microsoft/sql-server-samples/blob/master/samples/databases/northwind-pubs/instnwnd.sql) 

Task 1: Base API 

Create a Web API with the following resources: 

The Categories (api/categories). 
The Products (api/products). 


Both resources should support CRUD (create, read, update, delete) and list operations. 

Use https://www.postman.com for testing (or built-in swagger UI). 

Task 2: Add pagination and filtering for list operation for Product resource 

Extend Product’s list operation to accept the following parameters: pageNumber, pageSize, categoryId. Use these parameters for filtering the result of the operation. Parameters are optional with the following default values: pageNumber = 0 (or 1, depending on your Math 😊), pageSize = 10, categoryId = null (all categories). 

Task 3: Create a .NET REST API consumer 

Create a console app or a MS Test library with a REST Client, make a series of requests for your API.   
