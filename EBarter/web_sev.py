from zeep import Client


client4 = Client("http://localhost:44340/FruitInventory.asmx?WSDL")
response = client4.service.ListAllFruits();
print(response)



client5 = Client("http://localhost:44346/Ebarter.asmx?WSDL")
response = client5.service.BarterCalculater("Bartu Bozkurt","bHQ6XnpAkinzWgrEfbmt6Xzwl8w=","TV","ELMA",5000)
print(response)
factory = client5.type_factory('http://localhost/')
ac = factory.Acount(Name="Bartu Bozkurt",Key="bHQ6XnpAkinzWgrEfbmt6Xzwl8w=")
print(ac)
response = client5.service.BarterCalculater2(ac,"TV","ELMA",5000)
print(response)