USE `distribt`;

CREATE TABLE IF NOT EXISTS `Products` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(150) NOT NULL,
    `Description` varchar(150) NOT NULL,
    PRIMARY KEY (`Id`)
) AUTO_INCREMENT = 1;

INSERT IGNORE INTO `Products` (`Id`, `Name`, `Description`) VALUES
    (1, 'Producto 1', 'La descripción dice que es el primer producto'),
    (2, 'Segundo producto', 'Este es el producto número 2'),
    (3, 'Tercer producto', 'Terceras partes nunca fueron buenas');

CREATE TABLE IF NOT EXISTS `Orders` (
    `Id` varchar(36) NOT NULL,
    `Status` varchar(30) NOT NULL,
    `Street` varchar(250) NOT NULL,
    `City` varchar(100) NOT NULL,
    `Country` varchar(100) NOT NULL,
    `CardNumber` varchar(30) NOT NULL,
    `ExpireDate` varchar(10) NOT NULL,
    `Security` varchar(10) NOT NULL,
    `Version` int NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE IF NOT EXISTS `OrderProducts` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `OrderId` varchar(36) NOT NULL,
    `ProductId` int NOT NULL,
    `Quantity` int NOT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_OrderProducts_OrderId` (`OrderId`),
    CONSTRAINT `FK_OrderProducts_Orders_OrderId`
        FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS `OrderStatusHistory` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `OrderId` varchar(36) NOT NULL,
    `Status` varchar(30) NOT NULL,
    `Version` int NOT NULL,
    `OccurredAtUtc` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_OrderStatusHistory_OrderId` (`OrderId`),
    CONSTRAINT `FK_OrderStatusHistory_Orders_OrderId`
        FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE CASCADE
);
