# EFCoreQueryLab

EFCoreQueryLab, Entity Framework Core ve SQL sorgu mantýðýný pratik etmek için hazýrlanmýþ bir projesidir.

Bu projede EF Core’un sadece nasýl kullanýldýðýný deðil, arka planda SQL tarafýnda nasýl çalýþtýðýný anlamak amaçlanmýþtýr.

## Çalýþýlan Konular

* Tracking ve AsNoTracking
* Projection / Select kullanýmý
* Include ve ThenInclude
* EF Core sorgularýnýn SQL karþýlýðýný görme
* Filtering, sorting ve pagination
* GROUP BY ve HAVING
* INNER JOIN ve LEFT JOIN mantýðý
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

Bu proje, .NET backend geliþtirme sürecinde EF Core sorgularýný, SQL karþýlýklarýný ve performanslý veri çekme mantýðýný daha iyi anlamak için geliþtirilmiþtir.
