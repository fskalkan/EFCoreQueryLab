# EFCoreQueryLab

EFCoreQueryLab, Entity Framework Core ve SQL sorgu mantığını pratik etmek için hazırlanmış bir projedir.

Bu projede EF Core’un sadece nasıl kullanıldığını değil, arka planda SQL tarafında nasıl çalıştığını anlamak amaçlanmıştır.

## Çalışılan Konular

* Tracking ve AsNoTracking
* Projection / Select kullanımı
* Include ve ThenInclude
* EF Core sorgularının SQL karşılığını görme
* Filtering, sorting ve pagination
* GROUP BY ve HAVING
* INNER JOIN ve LEFT JOIN mantığı
* Object cycle problemini projection ile önleme

## Örnek Endpointler

```text
GET /api/queryexamples/products/tracking
GET /api/queryexamples/products/no-tracking
GET /api/queryexamples/products/projection
GET /api/queryexamples/products/sql
GET /api/queryexamples/products/filter-sort-page
GET /api/queryexamples/products/group-by-category
GET /api/queryexamples/orders/detail-projection
GET /api/queryexamples/orders/left-join-payment-sql
```

## Amaç

Bu proje, .NET backend geliştirme sürecinde EF Core sorgularını, SQL karşılıklarını ve performanslı veri çekme mantığını daha iyi anlamak için geliştirilmiştir.
