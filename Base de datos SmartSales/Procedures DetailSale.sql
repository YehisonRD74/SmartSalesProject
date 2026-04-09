USE SmartSales
GO


--Tomo como referencia
/*
--Tabla DetalleVenta
CREATE TABLE DetalleVenta(
    IdDetalle INT PRIMARY KEY IDENTITY (1,1),
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL (10,2) NOT NULL,
    IdVenta INT NOT NULL,
    IdProducto INT NOT NULL,
    FOREIGN KEY (IdVenta) REFERENCES Venta(IdVenta),
    FOREIGN KEY (IdProducto) REFERENCES Producto(IdProducto)
);
*/

--CRUD DetallesVentas
USE SmartSales
GO

CREATE PROCEDURE Sp_CreateDetalleVenta
	@IdProducto INT,
	@Cantidad INT,
	@PrecioUnitario DECIMAL(10,2), -- ¡Faltaba este!
	@Subtotal DECIMAL (10,2),
	@NuevoIdVenta INT -- Este ID ya viene con el número correcto desde C#
AS
BEGIN
	-- Insertamos directamente usando los datos que mandó C#
	INSERT INTO DetalleVenta (IdVenta, IdProducto, Cantidad, PrecioUnitario, SubTotal) 
	VALUES (@NuevoIdVenta, @IdProducto, @Cantidad, @PrecioUnitario, @Subtotal);
END;
GO


--Mostrar detalles de la venta
CREATE PROCEDURE Sp_GetDetallesByVenta
	@IdVenta INT
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT 
		u.Nombre AS NombreVendedor,
		c.Nombre AS NombreCliente,
		d.IdDetalle,
		d.IdProducto,
		p.Nombre AS NombreProducto,  -- Traemos el nombre del producto
		d.Cantidad,
		d.PrecioUnitario,
		d.SubTotal,
		v.Total
	FROM DetalleVenta d
	INNER JOIN Producto p ON d.IdProducto = p.IdProducto
	INNER JOIN Venta v ON v.IdVenta = d.IdVenta
	INNER JOIN Usuario u ON u.IdUsuario = v.IdUsuario
	INNER JOIN Cliente c ON c.IdCliente = v.IdCliente
	WHERE d.IdVenta = @IdVenta;
END;
GO
