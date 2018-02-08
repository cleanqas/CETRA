CREATE TABLE `roles` (
  `Id` varchar(128) NOT NULL,
  `Name` varchar(256) NOT NULL,
  PRIMARY KEY (`Id`)
);

CREATE TABLE `users` (
  `Id` varchar(128) NOT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext,
  `SecurityStamp` longtext,
  `PhoneNumber` longtext,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEndDateUtc` datetime DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int(11) NOT NULL,
  `UserName` varchar(256) NOT NULL,
  PRIMARY KEY (`Id`)
);

CREATE TABLE `userclaims` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` varchar(128) NOT NULL,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id` (`Id`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `ApplicationUser_Claims` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION
);

CREATE TABLE `userlogins` (
  `LoginProvider` varchar(128) NOT NULL,
  `ProviderKey` varchar(128) NOT NULL,
  `UserId` varchar(128) NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`,`UserId`),
  KEY `ApplicationUser_Logins` (`UserId`),
  CONSTRAINT `ApplicationUser_Logins` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION
);

CREATE TABLE `userroles` (
  `UserId` varchar(128) NOT NULL,
  `RoleId` varchar(128) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IdentityRole_Users` (`RoleId`),
  CONSTRAINT `ApplicationUser_Roles` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `IdentityRole_Users` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ;

ALTER TABLE `cetra`.`branches` 
CHANGE COLUMN `BranchName` `BranchName` VARCHAR(256) NOT NULL ,
ADD COLUMN `Bank` INT NOT NULL AFTER `BranchName`,
ADD COLUMN `GLAccount` VARCHAR(45) NOT NULL AFTER `Bank`;

ALTER TABLE `cetra`.`branches` 
CHANGE COLUMN `BankId` `BankId` VARCHAR(128) NOT NULL ;


CREATE TABLE `cetra`.`banks` (
  `Id` INT NOT NULL,
  `BankName` VARCHAR(256) NOT NULL,
  PRIMARY KEY (`Id`));

ALTER TABLE `cetra`.`banks` 
CHANGE COLUMN `Id` `Id` VARCHAR(128) NOT NULL ;

ALTER TABLE `cetra`.`branches` 
ADD INDEX `Branch_Bank_idx` (`BankId` ASC);
ALTER TABLE `cetra`.`branches` 
ADD CONSTRAINT `Branch_Bank`
  FOREIGN KEY (`BankId`)
  REFERENCES `cetra`.`banks` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

  CREATE TABLE `cetra`.`accountnumbers` (
  `Id` INT NOT NULL,
  `BankId` INT NOT NULL,
  `AccountNumber` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`Id`);

  ALTER TABLE `cetra`.`accountnumbers` 
CHANGE COLUMN `Id` `Id` VARCHAR(128) NOT NULL ,
CHANGE COLUMN `BankId` `BankId` VARCHAR(128) NOT NULL ,
ADD COLUMN `AccountName` VARCHAR(256) NULL AFTER `AccountNumber`;

ALTER TABLE `cetra`.`accountnumbers` 
ADD INDEX `AccountNumber_Bank_idx` (`BankId` ASC);
ALTER TABLE `cetra`.`accountnumbers` 
ADD CONSTRAINT `AccountNumber_Bank`
  FOREIGN KEY (`BankId`)
  REFERENCES `cetra`.`banks` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `cetra`.`userbranches` 
DROP PRIMARY KEY,
ADD PRIMARY KEY (`UserId`);

CREATE TABLE `cetra`.`uploadstatus` (
  `Id` INT NOT NULL,
  `Descr` VARCHAR(256) NOT NULL,
  PRIMARY KEY (`Id`));

  CREATE TABLE `cetra`.`uploads` (
  `Id` VARCHAR(128) NOT NULL,
  `UploaderId` VARCHAR(128) NOT NULL,
  `BranchId` VARCHAR(128) NOT NULL,
  `Status` INT NULL,
  PRIMARY KEY (`Id`),
  INDEX `Upload_User_idx` (`UploaderId` ASC),
  INDEX `Upload_Branch_idx` (`BranchId` ASC),
  INDEX `Upload_UploadStatus_idx` (`Status` ASC),
  CONSTRAINT `Upload_User`
    FOREIGN KEY (`UploaderId`)
    REFERENCES `cetra`.`users` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `Upload_Branch`
    FOREIGN KEY (`BranchId`)
    REFERENCES `cetra`.`branches` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `Upload_UploadStatus`
    FOREIGN KEY (`Status`)
    REFERENCES `cetra`.`uploadstatus` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

	CREATE TABLE `cetra`.`uploadsdata` (
  `Id` BIGINT(20) NOT NULL,
  `Narration` VARCHAR(256) NULL,
  `Amount` DECIMAL(18,2) NULL,
  `AccountNumber` VARCHAR(45) NULL,
  PRIMARY KEY (`Id`));

  ALTER TABLE `cetra`.`uploadsdata` 
ADD COLUMN `UploadId` VARCHAR(128) NOT NULL AFTER `Id`,
ADD INDEX `Uploaddata_upload_idx` (`UploadId` ASC);
ALTER TABLE `cetra`.`uploadsdata` 
ADD CONSTRAINT `Uploaddata_upload`
  FOREIGN KEY (`UploadId`)
  REFERENCES `cetra`.`uploads` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


ALTER TABLE `cetra`.`uploadsdata` 
CHANGE COLUMN `Id` `Id` BIGINT NOT NULL AUTO_INCREMENT;

INSERT INTO `cetra`.`uploadstatus` (`Id`, `Descr`) VALUES ('0', 'Pending');
INSERT INTO `cetra`.`uploadstatus` (`Id`, `Descr`) VALUES ('1', 'Processed');


ALTER TABLE `cetra`.`uploads` 
ADD COLUMN `UploadDate` DATETIME NULL DEFAULT CURRENT_TIMESTAMP AFTER `Status`;

ALTER TABLE `cetra`.`uploadsdata` 
CHANGE COLUMN `Id` `Id` VARCHAR(128) NOT NULL ;

ALTER TABLE `cetra`.`uploadsdata` 
ADD COLUMN `BankId` VARCHAR(128) NOT NULL AFTER `AccountNumber`;

ALTER TABLE `cetra`.`uploadsdata` 
ADD INDEX `Uploaddata_Bank_idx` (`BankId` ASC);
ALTER TABLE `cetra`.`uploadsdata` 
ADD CONSTRAINT `Uploaddata_Bank`
  FOREIGN KEY (`BankId`)
  REFERENCES `cetra`.`banks` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

  ALTER TABLE `cetra`.`uploadsdata` 
DROP FOREIGN KEY `Uploaddata_Bank`;
ALTER TABLE `cetra`.`uploadsdata` 
CHANGE COLUMN `BankId` `BankId` VARCHAR(128) NULL ;
ALTER TABLE `cetra`.`uploadsdata` 
ADD CONSTRAINT `Uploaddata_Bank`
  FOREIGN KEY (`BankId`)
  REFERENCES `cetra`.`banks` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

  ALTER TABLE `cetra`.`uploads` 
ADD COLUMN `OperatorId` VARCHAR(128) NULL AFTER `UploadDate`,
ADD COLUMN `RejectReason` VARCHAR(500) NULL AFTER `OperatorId`;

ALTER TABLE `cetra`.`uploads` 
ADD INDEX `UploadOperator_User_idx` (`OperatorId` ASC);
ALTER TABLE `cetra`.`uploads` 
ADD CONSTRAINT `UploadOperator_User`
  FOREIGN KEY (`OperatorId`)
  REFERENCES `cetra`.`users` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

  ALTER TABLE `cetra`.`uploads` 
ADD COLUMN `VerifierId` VARCHAR(128) NULL AFTER `RejectReason`;

ALTER TABLE `cetra`.`uploads` 
ADD COLUMN `HOProcessorId` VARCHAR(128) NULL AFTER `VerifierId`;

ALTER TABLE `cetra`.`uploads` 
ADD INDEX `UploadVerifier_User_idx` (`VerifierId` ASC);
ALTER TABLE `cetra`.`uploads` 
ADD CONSTRAINT `UploadVerifier_User`
  FOREIGN KEY (`VerifierId`)
  REFERENCES `cetra`.`users` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  

  ALTER TABLE `cetra`.`uploads` 
ADD INDEX `UploadHOProcessor_User_idx` (`HOProcessorId` ASC);
ALTER TABLE `cetra`.`uploads` 
ADD CONSTRAINT `UploadHOProcessor_User`
  FOREIGN KEY (`HOProcessorId`)
  REFERENCES `cetra`.`users` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


  UPDATE `cetra`.`uploadstatus` SET `Descr`='Operated' WHERE `Id`='1';
INSERT INTO `cetra`.`uploadstatus` (`Id`, `Descr`) VALUES ('2', 'Verified');
INSERT INTO `cetra`.`uploadstatus` (`Id`, `Descr`) VALUES ('3', 'Processed');
INSERT INTO `cetra`.`uploadstatus` (`Id`, `Descr`) VALUES ('-1', 'Rejected');


==============================Modifications after beta version
ALTER TABLE `cetra`.`branches` 
DROP FOREIGN KEY `Branch_Bank`;
ALTER TABLE `cetra`.`branches` 
DROP COLUMN `BankId`,
DROP INDEX `Branch_Bank_idx` ;

ALTER TABLE `cetra`.`uploads` 
ADD COLUMN `BankId` VARCHAR(128) NOT NULL AFTER `UploaderId`;

ALTER TABLE `cetra`.`uploads` 
ADD CONSTRAINT `Upload_Bank`
  FOREIGN KEY (`BankId`)
  REFERENCES `cetra`.`banks` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `cetra`.`branches` 
ADD COLUMN `BranchCode` VARCHAR(45) NOT NULL AFTER `GLAccount`;


UPDATE `cetra`.`branches` SET `BranchCode`='ADENIJI01' WHERE `Id`='04878a8c-3c27-4d48-9ed8-c354b68b17b5';
UPDATE `cetra`.`branches` SET `BranchCode`='HO001' WHERE `Id`='5a898810-3180-493d-9901-2dba066a74d1';

ALTER TABLE `cetra`.`uploadsdata` 
ADD COLUMN `Debit1Credit0` TINYINT(1) NULL AFTER `AccountNumber`,
ADD COLUMN `PostingCode` VARCHAR(45) NULL AFTER `Debit1Credit0`;

ALTER TABLE `cetra`.`uploadsdata` 
CHANGE COLUMN `Debit1Credit0` `DebitOrCredit` TINYINT(1) NULL DEFAULT NULL ;
