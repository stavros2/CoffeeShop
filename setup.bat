@echo off
echo -------------------------
echo Setting up CoffeeOrderAPI
echo -------------------------
echo(
echo ---------------------------------------------------
echo Restoring Dependancies and Building the Application
echo ---------------------------------------------------
cd CoffeeOrderAPI
dotnet restore
dotnet build

echo ----------------------------
echo Applying Database Migrations
echo ----------------------------

dotnet ef database update

echo ---------------------------------
echo Database setup completed.
echo ---------------------------------
echo(
echo ---------------------------------------------------------
echo Build Completed. Use run.bat to launch the application
echo ----------------------------------------------------------
