# Installing PostgreSQL for debugging purposes
As the project now requires PostgreSQL to function properly, it is now required to have a local database for ModCore 
to play with when debugging.

## Installing PostgreSQL server
ModCore is built and tested against PostgreSQL server 9.6. Any below version is not guaranteed to work. If you have a 
device like Raspberry Pi, or a Virtual Machine, it might be a good idea to use it as the PostgreSQL server host.

### Microsoft™ Windows™
To install PostgreSQL server on Windows-based systems, follow this procedure:

1.  Download official Windows distribution of PostgreSQL server 9.6 or newer from 
    [the release page](https://www.enterprisedb.com/downloads/postgres-postgresql-downloads#windows).
2.  Install the server using the installer. Make sure to note down the target directory and the superuser password.
3.  Open System Properties. This can be done by right-clicking Computer from Windows Explorer, and selecting Properties.
4.  Open Advanced System Settings. This is on the left pane.
5.  Go to Advanced tab, and open Environment Variables.
6.  In System Variables, locate PATH, and double-click it to open the editor.
7.  Add C:\Program Files\PostgreSQL\9.6\bin to PATH. If you installed PostgreSQL elsewhere, make sure to replace 
    C:\Program Files\PostgreSQL\9.6 with your actual installation path. Note that \bin must remain.
    * If using Windows 10, press New in the Environment Variable Editor. Enter the path in newly-created entry. Then 
      press OK to save the changes.
    * If using Windows 8.1 or older, go to Value box, press END on your keyboard, insert a semicolon (;), and enter the 
      path. Then press OK to save the changes.
8.  Press OK to exit from all windows. If you do not intend to connect to the server remotely, this is all you need to 
    do.
9.  Go to your PostgreSQL server installation directory, and locate data directory. Open a text editor as administrator.
10. Open the pg_hba.conf file. At the end of it, append the following text:

    ```
	host		all		all		192.168.1.0/24		md5
	```

	Of course, replace `192.168.1.0/24` with your own IP range.
11. Open Service Management, find postgresql-x64-9.6, and restart it. Depending on several factors, you might need to 
    configure your firewall to allow PostgreSQL connections.

### GNU/Linux
PostgreSQL server can be usually found in your distribution's package repository. If that is not the case, or the 
version is older than 9.6, you can download a generic Linux distribution of PostgreSQL server from 
[the Linux release page](https://www.enterprisedb.com/downloads/postgres-postgresql-downloads#linux).

The following instructions apply to Debian GNU/Linux, however they might be similar for other distributions. When in 
doubt, consult the documentation for your GNU/Linux distribution. The editor used here is GNU Nano.

1.  Install PostgreSQL server and client 9.6 (`sudo apt-get install postgresql-9.6 postgresql-client-9.6`).
2.  Login to PostgreSQL administrator account (`sudo -u postgres psql`).
3.  You will be dropped into psql command line. Set the user's password (`\password`). You will be asked for new 
    password twice. After this is done, exit (`\q`).
4.  Open PostgreSQL authentication config (`sudo nano /etc/postgresql/9.6/main/pg_hba.conf`).
5.  Locate the following lines: 

    ```
    local	all		all		peer
    local	all		postgres	peer
    ```
 
    Replace `peer` with `md5`.
6.  If your PostgreSQL server is not installed on the same machine as ModCore, append the following line to the end:
 
    ```
    host		all		all		192.168.1.0/24		md5
    ```
 
    Of course, replace `192.168.1.0/24` with your own IP range.
7.  Save the config by pressing Ctrl+O, then exit the editor by doing Ctrl+X.
8.  Open the main Postgres configuration file (`sudo nano /etc/postgresql/9.6/main/postgresql.conf`).
9.  Find `listen_address` (press Ctrl+W, and type that in, then press enter). You should find a line which looks like 
    so:
 
    ```
    #listen_address = 'localhost'
    ```
 
    Remove the #, and if you connect over network, replace `localhost` with `*`.
10. Save the config by pressing Ctrl+O, then exit the editor by doing Ctrl+X.
11. Restart PostgreSQL server (`sudo service postgresql restart`).

## Configuring PostgreSQL server
Once you finish installing PostgreSQL server on the target machine, you need to set up a database, and a user for 
ModCore to connect as. Furthermore, you will need to set up the schema.

Here, we assume the installation happened on the same host as the one you will be configuring it from. If that is not 
the case, replace `localhost` with appropriate hostname.

Start Command Prompt, PowerShell, or a terminal emulator, navigate to ModCore local repository rool, and follow these 
instructions:

1. Log in to the PostgreSQL server using the administrative user (`psql -U postgres -d postgres -h localhost`). If this 
   succeeds, you should be dropped into `psql` command prompt. If not, check your configuration and retry.
2. Create a new user called `modcore` and a password of your choosing 
   (`create user modcore with nocreaterole nocreatedb encrypted password 'enter password here';`).
3. Note down these credentials, you will need them later.
4. Create a new database for the `modcore` user, also called `modcore` 
   (`create database modcore with owner='modcore';`).
5. Exit the psql prompt (`\q`).
6. Install the schema. You need to set the PGPASSWORD environment variable for this to work:
   * Using Microsoft™ Windows™ and Command Prompt:
      * Drop into local environment (`setlocal`).
	  * Set the PGPASSWORD variable, and its value to your `modcore` user's password 
	    (`set PGPASSWORD=enter password here`).
      * Install the schema (`psql -U modcore -d modcore -h localhost < db_schema.sql`).
   * Using Microsoft™ Windows™ and PowerShell:
      * Set the environment variable (`$Env:PGPASSWORD="enter password here"`).
	  * Install the schema (`Get-Content db_schema.sql | psql -U modcore -d modcore -h localhost`).
   * Using GNU/Linux and Bash:
      * Install the schema using temporary PGPASSWORD variable 
	  (`PGPASSWORD="enter password here" psql -U modcore -d modcore -h localhost < db_schema.sql`).

## Configuring ModCore
Once your PostgreSQL server is installed and configured, you need to enter appropriate connection details. Open your 
settings.json, and make the following changes to the database section:

* Change hostname to match your PostgreSQL server hostname. If the PostgreSQL server is installed on the same machine 
  as ModCore will be running on, set it to localhost. Otherwise enter proper IP address or hostname.
* If you configured your PostgreSQL to run on non-default TCP port, enter this port here. Otherwise, leave this value 
  be.
* Set database to the name of the database you created for ModCore. If you followed the above instructions to the 
  letter, set it to modcore. Otherwise, enter the name you're using for your database.
* Set username to the name of the user you created for ModCore. Again, if you followed instructions to the letter, it's 
  going to be modcore.
* Set password to the password you entered for your database user.

Once this is all done, save the file, and debug the bot to see if it connects successfully.
