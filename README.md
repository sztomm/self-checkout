**Self Checkout**
=================

# Introduction
**Self Checkout** is a simulation of a supermarket self-checkout machine.

This have the following features:
- Oportunity to query the stocked bills and coins from the database
- Oportunity to import an amount of bills and coins into the database
- Functionality to calculate the money back for a given price

# Projects in Solution
|**Project name**|**.Net version**|**Purpose**|
|--|--|--|
|  SelfCheckout.API | .NET 6.0 web application | API for data handling |
|  SelfCheckout.DAL | .NET 6.0 | data access layer project  |

# Project Dependencies
- API -> DAL

# Prerequisites
- Visual Studio 2022
- .NET 6
- SQL Server 2016+

# Configuration
In the SelfCheckout.API it can be found **2** configuration files.

First is the appsettings.json file where we can handle the following configuration:
- Connection string - over this we can setup a database connection to our application

Second is the moneys.json file where we can edit currently the HUF curreny bills and coins values.
There the Type it can be **0** for coins and **1** for bills 

# Build & Run
The developed application it can be run and used from a Visual Studio 2022 or published under an IIS