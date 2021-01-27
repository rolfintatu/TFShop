# TF Shop
Is a shopping web application which provide a simple and easy to understand interface. It can be accessed at [TFShop](https://victorious-island-0da5b1403.azurestaticapps.net).

![Azure Static Web Apps CI/CD](https://github.com/rolfintatu/TFShop/workflows/Azure%20Static%20Web%20Apps%20CI/CD/badge.svg?branch=main)

## Progress

- [x] Seed Products (a simple methot that add some products in the storage)
- [x] Basket Creation (the basket is crested whe you add for first time an item in it)
- [x] Add items into basket
- [x] Remove items form basket
- [x] Change quantity for the items from basket (if the quantity is equal to 0 the item will be removed from basket)
- [x] Calculate subtotal and total with VAT
- [ ] Discount on basket (in case a client have more then ... different products into basket will benefit from a discount)
- [ ] Add authentication via Google Account (for those who are interested in this topic can read more about it [here](https://docs.microsoft.com/en-us/azure/static-web-apps/authentication-authorization))
- [ ] Add products (at this point I will migrate to a SQL database)


## The scope of this application
I've started this application to learn and deepen:

- Azure Functions;
- Azure Table Storage;
- Blazor and Local Storage;
- Manage a Static Web App on Azure;
- User Git and GitHub Actions.

## Challenges

- First challenge for me was basket creation. First time when I **implement** this was something like this: when a client reach the site a basket will be created and a reference (GUID) to his basket will be stored in local storage. But the **problem** was: if the client did not want to buy and he was searching for informations about a product, then the batabase will be populated with unnecessary data. My **solution** was: if the client want to buy something he will add that product to his basket, so I added basket creation when a product is added to the basket. I konw, with this solution I violated the Single Responsability Principle.

