# Contributing
Opening a pull request or issue? Please follow these guidelines, they aren't very complex.

## Pull Requests
Please specify:
- Changes made
- Whether it builds
- Whether it runs
- What has been tested
- How it has been tested

Make sure to add migrations if you made changes to the database:
- add a migration: `dotnet-ef add migration MigrationName`
- generate a config file and fill in details: `dotnet ModCore.dll --generate-configs`
- test the migration via the command line: `dotnet-ef database update`
- revert migration via ModCore's command line arguments: `dotnet ModCore.dll --rollback-one`
- test the migration via ModCore's command line arguments: `dotnet ModCore.dll --migrate`

# Issues
Please specify:
- Changes requested
- Problems found
- If applicable, a proposal on how to implement it