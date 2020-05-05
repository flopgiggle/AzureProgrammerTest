# Azure Functions App Test Description

This test usually takes one to two hours to finish if you have the experience. 

You have up to five hours to finish it from the time we send this test description link to you. 

Email your code to cloud-programmer@outlook.com before the deadline. Make sure the code file includes the script to creat the database. If you use Visual Studio develop the project, zip the whole solution and email the zip file.

If you can finsh the optional ["Publish this Funciton App to Azure"](#publish-this-funciton-app-to-azure-optional), send the API link with your code together. 

## Use C# to create an Azure Functions App with the following two Web APIs.

### 1. PostOrder

This API acceptes a http post request and creates the order in SQL database

- Order Structure  

    An order has four fields, BuyerName, PurchseOrderNumber, BillingZipCode, OrderAmount.  

    Here is a json sample: 
    
    ``` json 
    {
	    "BuyerName": "Test User 04",
	
	    "PurchaseOrderNumber" : "Test PO 04",

	    "BillingZipCode" : "40001",

	    "OrderAmount" : 49.99
    }
    ```

- If PurchaseOrderNumber exists in the database, return http status code 204.

- If any field is missing, reject it with a http status code 400.

- If the order is created successfully, return http status code 200.

- Sample

    Send post request to this url  
     https://azureprogrammertest.azurewebsites.net/api/PostOrder?code=kuIRSoezYc/IpAVszJZz2jDlxiywOjnIg4uHYaA1aM1A6v9xeH0tCQ== <br /><br />
        
    - PurchseOrderNumber exists in the database, return http status code 204
    
        ![204 sample](image/204.png)

    - BillingZipCode is missing, reject it with http status code 400
        ![400 sample](image/400.png)

    - Order is created successfully, return http status code 200
        ![200 sample](image/200.png)


### 2. GetOrders
This API acceptes a http get request and return the list of matched orders. Filters on BuyerName, PurchaseOrderNum and BillingZipCode should also be implemented.

- Sample
    Send GET request to this url  
    https://azureprogrammertest.azurewebsites.net/api/GetOrders?code=kuIRSoezYc/IpAVszJZz2jDlxiywOjnIg4uHYaA1aM1A6v9xeH0tCQ== <br /><br />

    - Without any filter, returns all orders in the database
        ![No Filter](image/NoFilter.png)
    - Apply filter on BuyName, only returns the matched record.
        ![Filter BuyerName](image/FilterBuyerName.png)
    - Apply filter on BillingZipCode, only returns the matched record.
        ![Filter BillingZipCode](image/FilterBillingZipCode.png)




## Publish this Funciton App to Azure (Optional)

Once you publish this Azure Fuctions App, send the Web API link to cloud-programmer@outlook.com with your code together.

```diff
- Do not send your code and Web API link separately. Both are counted on your total test time.
- If you don't know how to publish, just send the code.
```