SELECT * FROM products;
SET IDENTITY_INSERT Products ON

INSERT INTO products (
	ProductId,
	ProductName,
	DateSearched,
	ProductPrice)
VALUES (
	1,
	'Ryzen 9 5950X',
	'2023-10-22 12:26',
	'$ 356.75'
	);

TRUNCATE TABLE products;