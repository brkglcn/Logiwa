# Logiwa

### Kurulum

Merhabalar,

Projemiz **.Net Core 3.1** ile çalışmaktadır ve **Mongo veritabanı** kullanılmıştır.

Öncelikle ana dizinde bulunan "digiturk.json" dosyası Mongo veritabanına import edilmelidir. 

Bunun için:

> mongorestore -d Logiwa C:\logiwa.json\Logiwa

İsterseniz ana dizinde bulunan "postman_collection.json" ile endpoint'lere ve request'lere bakabilirsiniz.

### Cevaplar

- Projede veritabanı CRUD işlemleri için "Factory Method Pattern" kullandım. CRUD işlemleri yapılırken, kodu büyük ölçüde kısaltarak daha iyi anlaşılmasını sağlıyor. Genel olarak "Repository Pattern"e benzeyen bir yapı kullandım. "Controller", "Service" ve "Model"ler birbirinden ayrı olarak kurgulandı. "Controller" tarafı sadece Requestleri karşılarken "Service" tarafı hem CRUD işlemlerini düzenli tuttuğu gibi, klasör yapısını daha anlaşılır hale getirmektedir. "Model" tarafında ise "MongoEntities", "Requests" ve "Responses" klasörleri ile daha basitleştirilmiştir.

- Mongo Database'e erişim ve işlemler için "MongoDb.Driver" kütüphanesini kullandım. Ayrıca datalarım için "Microsoft.AspNetCore.Mvc.NewtonsoftJson" kütüphanesini kullandım. Bu kütüphaneler ile veriler ile daha iyi transactionlar alabileceğimi düşündüm.

- Daha geniş vaktim olsaydı:

  - Projeye, IdentityServer 4 ile "Authenticator" eklerdim böylece daha güvenli bir yapı elde ederdim.
  - Endpointler hakkında bilgilendirme amaçlı **Swagger** dokümanı eklerdim.
  - Modellere daha fazla "Property" eklerdim. Örneğin; **"Created Date"**, **"Updated Date"**, **"Session Data"**, **"Status"** gibi datalar ile daha detaylı bir yapı elde ederdim.
  - Angular 8 ile bir ön yüz oluştururdum.

# Teşekkürler




