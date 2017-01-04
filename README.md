# PortfolioRestToken

This is a .NET Core C# implementation of the [Portfolio 2016 REST API + API Token Authentication](http://labs.extensis.com/viewtopic.php?f=11&t=355) demo (written in Python) provided by Extensis.

## Preliminaries

- HTTP (or HTTPS) access to a Portfolio 2016 server that is licensed for the Portfolio API.
  - e.g. http://portfolio.example.com:8090 or https://portfolio.example.com:9443 _(HTTPS is strongly recommended if connecting via API Token to a Portfolio 2016 server over the Internet.)_
- An API Token (alphanumeric sequence of characters that begins with `TOKEN-`).
  - Within the Portfolio Admin Web interface, select "Users" > "Add new API Token."
- API Token has been granted a minimum of Reader level access to at least one Portfolio Catalog, Catalog contains multiple assets, and assets have been tagged with Keywords.

## Portfolio API Documentation

Visit this [Extensis Labs page](http://labs.extensis.com/viewtopic.php?f=11&t=368&p=978) to download the Swagger documentation for the Portfolio API.
