IF DB_ID(N'SmartSales') IS NULL
BEGIN
    CREATE DATABASE [SmartSales];
END;
GO

USE [SmartSales];
GO

IF OBJECT_ID('dbo.Cliente','U') IS NULL
BEGIN
    CREATE TABLE dbo.Cliente
    (
        IdCliente INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nombre NVARCHAR(150) NOT NULL,
        Email NVARCHAR(200) NULL,
        Telefono NVARCHAR(50) NULL
    );
END;
GO

IF OBJECT_ID('dbo.Producto','U') IS NULL
BEGIN
    CREATE TABLE dbo.Producto
    (
        IdProducto INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nombre NVARCHAR(150) NOT NULL,
        Precio DECIMAL(18,2) NOT NULL,
        Stock INT NOT NULL
    );
END;
GO

IF OBJECT_ID('dbo.Usuario','U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuario
    (
        IdUsuario INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nombre NVARCHAR(150) NOT NULL,
        Email NVARCHAR(200) NULL,
        ContrasenaHash NVARCHAR(300) NULL,
        Rol NVARCHAR(50) NULL,
        Estado BIT NOT NULL CONSTRAINT DF_Usuario_Estado DEFAULT(1)
    );
END;
GO

IF OBJECT_ID('dbo.Venta','U') IS NULL
BEGIN
    CREATE TABLE dbo.Venta
    (
        IdVenta INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdCliente INT NOT NULL,
        IdUsuario INT NOT NULL,
        Fecha DATETIME2 NOT NULL CONSTRAINT DF_Venta_Fecha DEFAULT(SYSDATETIME()),
        Total DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_Venta_Cliente FOREIGN KEY (IdCliente) REFERENCES dbo.Cliente(IdCliente),
        CONSTRAINT FK_Venta_Usuario FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuario(IdUsuario)
    );
END;
GO

IF OBJECT_ID('dbo.DetalleVenta','U') IS NULL
BEGIN
    CREATE TABLE dbo.DetalleVenta
    (
        IdDetalle INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdVenta INT NOT NULL,
        IdProducto INT NOT NULL,
        Cantidad INT NOT NULL,
        PrecioUnitario DECIMAL(18,2) NOT NULL,
        SubTotal DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_DetalleVenta_Venta FOREIGN KEY (IdVenta) REFERENCES dbo.Venta(IdVenta) ON DELETE CASCADE,
        CONSTRAINT FK_DetalleVenta_Producto FOREIGN KEY (IdProducto) REFERENCES dbo.Producto(IdProducto)
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_CreateCustomer
    @Nombre NVARCHAR(150),
    @Email NVARCHAR(200) = NULL,
    @Telefono NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Cliente(Nombre, Email, Telefono)
    VALUES(@Nombre, @Email, @Telefono);
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_UpdateCustomer
    @IdCliente INT,
    @Nombre NVARCHAR(150),
    @Email NVARCHAR(200) = NULL,
    @Telefono NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Cliente
    SET Nombre = @Nombre,
        Email = @Email,
        Telefono = @Telefono
    WHERE IdCliente = @IdCliente;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_DeleteCustomer
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Cliente WHERE IdCliente = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_GetAllCustomers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdCliente, Nombre, Email, Telefono
    FROM dbo.Cliente
    ORDER BY IdCliente DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_GetCustomerById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdCliente, Nombre, Email, Telefono
    FROM dbo.Cliente
    WHERE IdCliente = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_GetCustomersByName
    @Nombre NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdCliente, Nombre, Email, Telefono
    FROM dbo.Cliente
    WHERE Nombre LIKE '%' + @Nombre + '%'
    ORDER BY Nombre;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_CreateProduct
    @Nombre NVARCHAR(150),
    @Precio DECIMAL(18,2),
    @Stock INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Producto(Nombre, Precio, Stock)
    VALUES(@Nombre, @Precio, @Stock);
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_UpdateProduct
    @Id INT,
    @Nombre NVARCHAR(150) = NULL,
    @Precio DECIMAL(18,2) = NULL,
    @Stock INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Producto
    SET Nombre = COALESCE(@Nombre, Nombre),
        Precio = COALESCE(@Precio, Precio),
        Stock = COALESCE(@Stock, Stock)
    WHERE IdProducto = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_DeleteProduct
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Producto WHERE IdProducto = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_GetAllProducts
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdProducto, Nombre, Precio, Stock
    FROM dbo.Producto
    ORDER BY IdProducto DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_GetProductById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdProducto, Nombre, Precio, Stock
    FROM dbo.Producto
    WHERE IdProducto = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_GetProductByName
    @Nombre NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdProducto, Nombre, Precio, Stock
    FROM dbo.Producto
    WHERE Nombre LIKE '%' + @Nombre + '%'
    ORDER BY Nombre;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_CreateUser
    @Nombre NVARCHAR(150),
    @Email NVARCHAR(200) = NULL,
    @Contraseña NVARCHAR(300) = NULL,
    @Rol NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Usuario(Nombre, Email, ContrasenaHash, Rol, Estado)
    VALUES(@Nombre, @Email, @Contraseña, @Rol, 1);
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_UpdateUser
    @Id INT,
    @Nombre NVARCHAR(150),
    @Email NVARCHAR(200) = NULL,
    @Contrasena NVARCHAR(300) = NULL,
    @Rol NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Usuario
    SET Nombre = @Nombre,
        Email = @Email,
        ContrasenaHash = @Contrasena,
        Rol = @Rol
    WHERE IdUsuario = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_setUserStatus
    @Id INT,
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Usuario
    SET Estado = CASE WHEN Estado = 1 THEN 0 ELSE 1 END
    WHERE IdUsuario = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_GetAllUser
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdUsuario, Nombre, Email, Rol, Estado
    FROM dbo.Usuario
    ORDER BY IdUsuario DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_GetUserById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdUsuario, Nombre, Email, Rol, Estado
    FROM dbo.Usuario
    WHERE IdUsuario = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_GetUserByName
    @Nombre NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdUsuario, Nombre, Email, Rol, Estado
    FROM dbo.Usuario
    WHERE Nombre LIKE '%' + @Nombre + '%'
    ORDER BY Nombre;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_CreateVenta
    @IdCliente INT,
    @IdUsuario INT,
    @Total DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Venta(IdCliente, IdUsuario, Total)
    VALUES(@IdCliente, @IdUsuario, @Total);

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_CreateDetalleVenta
    @NuevoIdVenta INT,
    @IdProducto INT,
    @Cantidad INT,
    @PrecioUnitario DECIMAL(18,2),
    @Subtotal DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.DetalleVenta(IdVenta, IdProducto, Cantidad, PrecioUnitario, SubTotal)
    VALUES(@NuevoIdVenta, @IdProducto, @Cantidad, @PrecioUnitario, @Subtotal);
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_GetAllVentas
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        v.IdVenta,
        v.Fecha,
        v.Total,
        v.IdCliente,
        c.Nombre AS NombreCliente,
        v.IdUsuario,
        u.Nombre AS NombreVendedor
    FROM dbo.Venta v
    INNER JOIN dbo.Cliente c ON c.IdCliente = v.IdCliente
    INNER JOIN dbo.Usuario u ON u.IdUsuario = v.IdUsuario
    ORDER BY v.IdVenta DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_GetVentaById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        v.IdVenta,
        v.Fecha,
        v.Total,
        v.IdCliente,
        c.Nombre AS NombreCliente,
        v.IdUsuario,
        u.Nombre AS NombreVendedor
    FROM dbo.Venta v
    INNER JOIN dbo.Cliente c ON c.IdCliente = v.IdCliente
    INNER JOIN dbo.Usuario u ON u.IdUsuario = v.IdUsuario
    WHERE v.IdVenta = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Sp_GetDetallesByVenta
    @IdVenta INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        dv.IdDetalle,
        dv.IdVenta,
        dv.IdProducto,
        p.Nombre AS NombreProducto,
        dv.Cantidad,
        dv.PrecioUnitario,
        dv.SubTotal,
        v.Total,
        c.Nombre AS NombreCliente,
        u.Nombre AS NombreVendedor
    FROM dbo.DetalleVenta dv
    INNER JOIN dbo.Venta v ON v.IdVenta = dv.IdVenta
    INNER JOIN dbo.Cliente c ON c.IdCliente = v.IdCliente
    INNER JOIN dbo.Usuario u ON u.IdUsuario = v.IdUsuario
    INNER JOIN dbo.Producto p ON p.IdProducto = dv.IdProducto
    WHERE dv.IdVenta = @IdVenta
    ORDER BY dv.IdDetalle;
END;
GO
