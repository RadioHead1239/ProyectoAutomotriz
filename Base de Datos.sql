IF DB_ID(N'TallerDB') IS NULL
    CREATE DATABASE [TallerDB];
GO

USE [TallerDB];
GO
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [Clientes] (
        [Id] int NOT NULL IDENTITY,
        [Documento] nvarchar(15) NOT NULL,
        [Nombre] nvarchar(150) NOT NULL,
        [Telefono] nvarchar(20) NULL,
        [Correo] nvarchar(150) NULL,
        CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [Proveedores] (
        [Id] int NOT NULL IDENTITY,
        [Ruc] nvarchar(11) NOT NULL,
        [RazonSocial] nvarchar(150) NOT NULL,
        [Contacto] nvarchar(150) NULL,
        CONSTRAINT [PK_Proveedores] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [Servicios] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [PrecioBase] decimal(10,2) NOT NULL,
        [Activo] bit NOT NULL,
        CONSTRAINT [PK_Servicios] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Servicios_PrecioBase] CHECK ([PrecioBase] >= 0)
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [Sucursales] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Direccion] nvarchar(200) NOT NULL,
        [Activa] bit NOT NULL,
        CONSTRAINT [PK_Sucursales] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [Vehiculos] (
        [Id] int NOT NULL IDENTITY,
        [Placa] nvarchar(10) NOT NULL,
        [Marca] nvarchar(50) NOT NULL,
        [Modelo] nvarchar(50) NOT NULL,
        [Anio] int NOT NULL,
        [ClienteId] int NOT NULL,
        CONSTRAINT [PK_Vehiculos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Vehiculos_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [Repuestos] (
        [Id] int NOT NULL IDENTITY,
        [Codigo] nvarchar(30) NOT NULL,
        [Nombre] nvarchar(150) NOT NULL,
        [Categoria] nvarchar(60) NOT NULL,
        [PrecioVenta] decimal(10,2) NOT NULL,
        [StockMinimo] int NOT NULL,
        [ProveedorId] int NULL,
        CONSTRAINT [PK_Repuestos] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Repuestos_StockMinimo] CHECK ([StockMinimo] >= 0),
        CONSTRAINT [FK_Repuestos_Proveedores_ProveedorId] FOREIGN KEY ([ProveedorId]) REFERENCES [Proveedores] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [OrdenesCompra] (
        [Id] int NOT NULL IDENTITY,
        [Fecha] datetime2 NOT NULL,
        [Estado] nvarchar(30) NOT NULL,
        [ProveedorId] int NOT NULL,
        [SucursalId] int NOT NULL,
        CONSTRAINT [PK_OrdenesCompra] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrdenesCompra_Proveedores_ProveedorId] FOREIGN KEY ([ProveedorId]) REFERENCES [Proveedores] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OrdenesCompra_Sucursales_SucursalId] FOREIGN KEY ([SucursalId]) REFERENCES [Sucursales] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [Usuarios] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Correo] nvarchar(150) NOT NULL,
        [ClaveHash] nvarchar(500) NOT NULL,
        [Rol] nvarchar(30) NOT NULL,
        [SucursalId] int NOT NULL,
        [Activo] bit NOT NULL,
        [FechaRegistro] datetime2 NOT NULL,
        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Usuarios_Sucursales_SucursalId] FOREIGN KEY ([SucursalId]) REFERENCES [Sucursales] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [StockSucursales] (
        [RepuestoId] int NOT NULL,
        [SucursalId] int NOT NULL,
        [Cantidad] int NOT NULL,
        [EnTransito] int NOT NULL,
        CONSTRAINT [PK_StockSucursales] PRIMARY KEY ([RepuestoId], [SucursalId]),
        CONSTRAINT [CK_StockSucursales_Cantidad] CHECK ([Cantidad] >= 0),
        CONSTRAINT [CK_StockSucursales_EnTransito] CHECK ([EnTransito] >= 0),
        CONSTRAINT [FK_StockSucursales_Repuestos_RepuestoId] FOREIGN KEY ([RepuestoId]) REFERENCES [Repuestos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_StockSucursales_Sucursales_SucursalId] FOREIGN KEY ([SucursalId]) REFERENCES [Sucursales] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [OrdenCompraDetalles] (
        [Id] int NOT NULL IDENTITY,
        [OrdenCompraId] int NOT NULL,
        [RepuestoId] int NOT NULL,
        [Cantidad] int NOT NULL,
        [CostoUnitario] decimal(10,2) NOT NULL,
        CONSTRAINT [PK_OrdenCompraDetalles] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_OrdenCompraDetalles_Cantidad] CHECK ([Cantidad] > 0),
        CONSTRAINT [FK_OrdenCompraDetalles_OrdenesCompra_OrdenCompraId] FOREIGN KEY ([OrdenCompraId]) REFERENCES [OrdenesCompra] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrdenCompraDetalles_Repuestos_RepuestoId] FOREIGN KEY ([RepuestoId]) REFERENCES [Repuestos] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [Auditoria] (
        [Id] int NOT NULL IDENTITY,
        [Fecha] datetime2 NOT NULL,
        [UsuarioId] int NULL,
        [Accion] nvarchar(50) NOT NULL,
        [Entidad] nvarchar(60) NOT NULL,
        [EntidadId] nvarchar(30) NULL,
        [Detalle] nvarchar(1000) NULL,
        CONSTRAINT [PK_Auditoria] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Auditoria_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [MovimientosKardex] (
        [Id] int NOT NULL IDENTITY,
        [Fecha] datetime2 NOT NULL,
        [Tipo] nvarchar(30) NOT NULL,
        [RepuestoId] int NOT NULL,
        [SucursalId] int NOT NULL,
        [Cantidad] int NOT NULL,
        [SaldoFinal] int NOT NULL,
        [Referencia] nvarchar(100) NULL,
        [UsuarioId] int NOT NULL,
        CONSTRAINT [PK_MovimientosKardex] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MovimientosKardex_Repuestos_RepuestoId] FOREIGN KEY ([RepuestoId]) REFERENCES [Repuestos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MovimientosKardex_Sucursales_SucursalId] FOREIGN KEY ([SucursalId]) REFERENCES [Sucursales] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MovimientosKardex_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [OrdenesTrabajo] (
        [Id] int NOT NULL IDENTITY,
        [Fecha] datetime2 NOT NULL,
        [Estado] nvarchar(30) NOT NULL,
        [Diagnostico] nvarchar(1000) NULL,
        [VehiculoId] int NOT NULL,
        [SucursalId] int NOT NULL,
        [MecanicoId] int NULL,
        CONSTRAINT [PK_OrdenesTrabajo] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrdenesTrabajo_Sucursales_SucursalId] FOREIGN KEY ([SucursalId]) REFERENCES [Sucursales] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OrdenesTrabajo_Usuarios_MecanicoId] FOREIGN KEY ([MecanicoId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OrdenesTrabajo_Vehiculos_VehiculoId] FOREIGN KEY ([VehiculoId]) REFERENCES [Vehiculos] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [TomasFisicas] (
        [Id] int NOT NULL IDENTITY,
        [Fecha] datetime2 NOT NULL,
        [SucursalId] int NOT NULL,
        [UsuarioId] int NOT NULL,
        [Estado] nvarchar(30) NOT NULL,
        CONSTRAINT [PK_TomasFisicas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TomasFisicas_Sucursales_SucursalId] FOREIGN KEY ([SucursalId]) REFERENCES [Sucursales] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TomasFisicas_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [Transferencias] (
        [Id] int NOT NULL IDENTITY,
        [Fecha] datetime2 NOT NULL,
        [Estado] nvarchar(30) NOT NULL,
        [SucursalOrigenId] int NOT NULL,
        [SucursalDestinoId] int NOT NULL,
        [SolicitanteId] int NOT NULL,
        CONSTRAINT [PK_Transferencias] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Transferencias_Sucursales] CHECK ([SucursalOrigenId] <> [SucursalDestinoId]),
        CONSTRAINT [FK_Transferencias_Sucursales_SucursalDestinoId] FOREIGN KEY ([SucursalDestinoId]) REFERENCES [Sucursales] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Transferencias_Sucursales_SucursalOrigenId] FOREIGN KEY ([SucursalOrigenId]) REFERENCES [Sucursales] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Transferencias_Usuarios_SolicitanteId] FOREIGN KEY ([SolicitanteId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [Comprobantes] (
        [Id] int NOT NULL IDENTITY,
        [Tipo] nvarchar(30) NOT NULL,
        [Serie] nvarchar(4) NOT NULL,
        [Correlativo] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [Total] decimal(10,2) NOT NULL,
        [OrdenTrabajoId] int NOT NULL,
        CONSTRAINT [PK_Comprobantes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Comprobantes_OrdenesTrabajo_OrdenTrabajoId] FOREIGN KEY ([OrdenTrabajoId]) REFERENCES [OrdenesTrabajo] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [OrdenRepuestos] (
        [Id] int NOT NULL IDENTITY,
        [OrdenTrabajoId] int NOT NULL,
        [RepuestoId] int NOT NULL,
        [Cantidad] int NOT NULL,
        [PrecioUnitario] decimal(10,2) NOT NULL,
        CONSTRAINT [PK_OrdenRepuestos] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_OrdenRepuestos_Cantidad] CHECK ([Cantidad] > 0),
        CONSTRAINT [FK_OrdenRepuestos_OrdenesTrabajo_OrdenTrabajoId] FOREIGN KEY ([OrdenTrabajoId]) REFERENCES [OrdenesTrabajo] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrdenRepuestos_Repuestos_RepuestoId] FOREIGN KEY ([RepuestoId]) REFERENCES [Repuestos] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [OrdenServicios] (
        [Id] int NOT NULL IDENTITY,
        [OrdenTrabajoId] int NOT NULL,
        [ServicioId] int NOT NULL,
        [Precio] decimal(10,2) NOT NULL,
        CONSTRAINT [PK_OrdenServicios] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrdenServicios_OrdenesTrabajo_OrdenTrabajoId] FOREIGN KEY ([OrdenTrabajoId]) REFERENCES [OrdenesTrabajo] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrdenServicios_Servicios_ServicioId] FOREIGN KEY ([ServicioId]) REFERENCES [Servicios] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [TokensAcceso] (
        [Id] int NOT NULL IDENTITY,
        [Token] nvarchar(100) NOT NULL,
        [Tipo] nvarchar(30) NOT NULL,
        [OrdenTrabajoId] int NULL,
        [UsuarioId] int NULL,
        [Vence] datetime2 NOT NULL,
        [Usado] bit NOT NULL,
        CONSTRAINT [PK_TokensAcceso] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TokensAcceso_OrdenesTrabajo_OrdenTrabajoId] FOREIGN KEY ([OrdenTrabajoId]) REFERENCES [OrdenesTrabajo] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TokensAcceso_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [TomaFisicaDetalles] (
        [Id] int NOT NULL IDENTITY,
        [TomaFisicaId] int NOT NULL,
        [RepuestoId] int NOT NULL,
        [CantidadSistema] int NOT NULL,
        [CantidadContada] int NOT NULL,
        [Diferencia] int NOT NULL,
        CONSTRAINT [PK_TomaFisicaDetalles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TomaFisicaDetalles_Repuestos_RepuestoId] FOREIGN KEY ([RepuestoId]) REFERENCES [Repuestos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TomaFisicaDetalles_TomasFisicas_TomaFisicaId] FOREIGN KEY ([TomaFisicaId]) REFERENCES [TomasFisicas] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE TABLE [TransferenciaDetalles] (
        [Id] int NOT NULL IDENTITY,
        [TransferenciaId] int NOT NULL,
        [RepuestoId] int NOT NULL,
        [Cantidad] int NOT NULL,
        CONSTRAINT [PK_TransferenciaDetalles] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_TransferenciaDetalles_Cantidad] CHECK ([Cantidad] > 0),
        CONSTRAINT [FK_TransferenciaDetalles_Repuestos_RepuestoId] FOREIGN KEY ([RepuestoId]) REFERENCES [Repuestos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TransferenciaDetalles_Transferencias_TransferenciaId] FOREIGN KEY ([TransferenciaId]) REFERENCES [Transferencias] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_Auditoria_Fecha] ON [Auditoria] ([Fecha]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_Auditoria_UsuarioId] ON [Auditoria] ([UsuarioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Clientes_Documento] ON [Clientes] ([Documento]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Comprobantes_OrdenTrabajoId] ON [Comprobantes] ([OrdenTrabajoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Comprobantes_Serie_Correlativo] ON [Comprobantes] ([Serie], [Correlativo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_MovimientosKardex_RepuestoId_SucursalId_Fecha] ON [MovimientosKardex] ([RepuestoId], [SucursalId], [Fecha]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_MovimientosKardex_SucursalId] ON [MovimientosKardex] ([SucursalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_MovimientosKardex_UsuarioId] ON [MovimientosKardex] ([UsuarioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_OrdenCompraDetalles_OrdenCompraId] ON [OrdenCompraDetalles] ([OrdenCompraId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_OrdenCompraDetalles_RepuestoId] ON [OrdenCompraDetalles] ([RepuestoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_OrdenesCompra_ProveedorId] ON [OrdenesCompra] ([ProveedorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_OrdenesCompra_SucursalId] ON [OrdenesCompra] ([SucursalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_OrdenesTrabajo_MecanicoId] ON [OrdenesTrabajo] ([MecanicoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_OrdenesTrabajo_SucursalId_Estado] ON [OrdenesTrabajo] ([SucursalId], [Estado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_OrdenesTrabajo_VehiculoId] ON [OrdenesTrabajo] ([VehiculoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_OrdenRepuestos_OrdenTrabajoId] ON [OrdenRepuestos] ([OrdenTrabajoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_OrdenRepuestos_RepuestoId] ON [OrdenRepuestos] ([RepuestoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_OrdenServicios_OrdenTrabajoId] ON [OrdenServicios] ([OrdenTrabajoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_OrdenServicios_ServicioId] ON [OrdenServicios] ([ServicioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Proveedores_Ruc] ON [Proveedores] ([Ruc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Repuestos_Codigo] ON [Repuestos] ([Codigo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_Repuestos_ProveedorId] ON [Repuestos] ([ProveedorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_StockSucursales_SucursalId] ON [StockSucursales] ([SucursalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Sucursales_Nombre] ON [Sucursales] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_TokensAcceso_OrdenTrabajoId] ON [TokensAcceso] ([OrdenTrabajoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TokensAcceso_Token] ON [TokensAcceso] ([Token]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_TokensAcceso_UsuarioId] ON [TokensAcceso] ([UsuarioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_TomaFisicaDetalles_RepuestoId] ON [TomaFisicaDetalles] ([RepuestoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_TomaFisicaDetalles_TomaFisicaId] ON [TomaFisicaDetalles] ([TomaFisicaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_TomasFisicas_SucursalId] ON [TomasFisicas] ([SucursalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_TomasFisicas_UsuarioId] ON [TomasFisicas] ([UsuarioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_TransferenciaDetalles_RepuestoId] ON [TransferenciaDetalles] ([RepuestoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_TransferenciaDetalles_TransferenciaId] ON [TransferenciaDetalles] ([TransferenciaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_Transferencias_SolicitanteId] ON [Transferencias] ([SolicitanteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_Transferencias_SucursalDestinoId] ON [Transferencias] ([SucursalDestinoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_Transferencias_SucursalOrigenId] ON [Transferencias] ([SucursalOrigenId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_Correo] ON [Usuarios] ([Correo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_Usuarios_SucursalId] ON [Usuarios] ([SucursalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE INDEX [IX_Vehiculos_ClienteId] ON [Vehiculos] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Vehiculos_Placa] ON [Vehiculos] ([Placa]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108081206_Inicial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261108081206_Inicial', N'10.0.14');
END;

COMMIT;
GO
