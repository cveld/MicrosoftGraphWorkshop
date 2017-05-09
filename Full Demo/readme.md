Lees ook de installatie-instructies op https://github.com/microsoftgraph/aspnet-webhooks-rest-sample

1. Required permissions:
* Read my groups
* Read my mail
* Read videos

2. Start ngrok op met poort 22116: 
ngrok.exe http 22116 -host-header=localhost:22116

3. Plak https://7217b4ba.ngrok.io in web.config bij ida:NotificationUrl




### SQLLocalDB troubleshooting
f you delete the DB file, it still stays registered with SqlLocalDB. Sometimes it fixes it to delete the DB. You can do this from the command line.

Open the "Developer Command Propmpt for VisualStudio" under your start/programs menu.
Run the following commands:

sqllocaldb.exe stop v11.0

sqllocaldb.exe delete v11.0