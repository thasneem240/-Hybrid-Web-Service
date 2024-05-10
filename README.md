# Hybrid-Web-Service / .NET Remoting system
# Contains 3 Main Parts

# An ASP.NET MVC Web Service
# A desktop application (with NET Remoting)
# An ASP.NET CORE Website

This project is a ASP.NET Core Web App (Model-View-Controller). The website has a single page that displays the relevant data 
from the local database created in the Web Server project, in the form of a table. 

The project has a model class called Clients that is used to store the information retrieved from the database. 

The project has a single controller called HomeController.cs . This controller uses a REST API GET call to retrieve all the 
records present in the database and fills a list called clientList with the retrieved data. 
The view then accesses the model list and renders the data in it in the form of a table. 