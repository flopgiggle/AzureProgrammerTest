dotnet ef dbcontext scaffold "server=rm-wz9d6e0npz8dfmmo4bo.mysql.rds.aliyuncs.com;uid=liwenhai;pwd=aaaa1qaz@WSX;database=Binggo;TreatTinyAsBoolean=true""Pomelo.EntityFrameworkCore.Mysql"-o MysqlModels

Scaffold-DbContext -Connection "Server=rm-wz9d6e0npz8dfmmo4bo.mysql.rds.aliyuncs.com;User Id=liwenhai;Password=aaaa1qaz@WSX;Database=Binggo;TreatTinyAsBoolean=true" -Provider "Pomelo.EntityFrameworkCore.MySql" -OutputDir "Models" -force -Context "Db"


部署linux 
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
sudo yum install dotnet-sdk-2.2

nohup dotnet Bingo.dll

#查找.net 进程
ps aux | grep dotnet

#杀死.net 进程
kill -9 你的进程ID