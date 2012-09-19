Windows Azure Table Storage ADO.NET Data Provider
=================================================
-------------------------------------------------
If you run into any trouble or have any suggestions, please log them in the [Issue Tracker](https://github.com/honkywater/TableStorageDataProvider/issues). Enjoy!

### Changelog

#### v1.0.2

* Added a DbConnectionExtensions class, which contains a helper method for creating a command with command text.
* Fixed a bug that was causing trouble on insert and update commands.

#### v1.0.1

* Added the DbCommandExtensions class, which contains helper methods for adding parameters to a DbCommand instance.

#### v1.0.0

* Implemented ADO.NET Data Provider classes to allow developers to write code to access Windows Azure Table Storage with ADO.NET classes like DbCommand and DbDataAdapter.