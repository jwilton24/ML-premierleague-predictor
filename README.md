> **Note:** This repository is a reconstruction of a previous project. The codebase was uploaded from the original documentation as a portfolio reference.

# Premier League Statistics Predictor (ML.NET)

A Windows Forms desktop application in C# that uses machine learning to predict future statistics (goals, assists, shots, and shots on target) for Premier League footballers. 

This project was developed to provide mathematically supported insights into player performance for Fantasy Premier League players and sports analysts. It features a custom backend built with SQL and a machine learning pipeline which uses ML.NET's SDCA Regression Trainer. The application was designed to prioritise backend engineering with the frontend serving mainly as an interface for functional testing.

## Key Features

* **Machine Learning Pipeline:** Implements a SDCA regression model using `Microsoft.ML` to predict continuous variables based on historical player data. Data is normalised before training to ensure stability.
* **Custom Algorithms:** Uses a custom, recursive Merge Sort algorithm to calculate and retrieve top goal-scorers and shot-takers from the dataset.
* **SQL Integration:** Connects to a local SQL Server database using `System.Data.SqlClient`. The database is structured in Third Normal Form (3NF) and is queried using SQL with aggregate functions such as (`SUM`) and table joins (`JOIN`).
* **Secure Authentication:** Uses a custom bitwise XOR hashing algorithm to process and store user passwords.
* **Data Visualisation:** Creates line charts using `System.Windows.Forms.DataVisualization.Charting` to display recent player performance trends to the user.

## Technical Stack

* **Language:** C#
* **Framework:** .NET Framework (Windows Forms)
* **Database:** SQL Server LocalDB (`System.Data.SqlClient`)
* **Machine Learning:** ML.NET (`Microsoft.ML`)

<img width="300" height="225" alt="search" src="https://github.com/user-attachments/assets/a97c7434-f0ab-4f61-b16b-fc2bfe1de7dd" />
<img width="400" height="225" alt="prediction" src="https://github.com/user-attachments/assets/f9772f2a-83d9-40eb-933d-fe74a49d613f" />
<img width="300" height="225" alt="comparison" src="https://github.com/user-attachments/assets/38d99e54-d673-47a5-967e-9f31c777c3f7" />
