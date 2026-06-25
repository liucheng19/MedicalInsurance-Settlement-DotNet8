-- MySQL dump 10.13  Distrib 8.0.46, for Win64 (x86_64)
--
-- Host: localhost    Database: nacos_config
-- ------------------------------------------------------
-- Server version	8.0.46

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `his_config_info`
--

DROP TABLE IF EXISTS `his_config_info`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `his_config_info` (
  `id` bigint unsigned NOT NULL COMMENT 'id',
  `nid` bigint unsigned NOT NULL AUTO_INCREMENT COMMENT 'nid, 自增标识',
  `data_id` varchar(255) COLLATE utf8mb3_bin NOT NULL COMMENT 'data_id',
  `group_id` varchar(128) COLLATE utf8mb3_bin NOT NULL COMMENT 'group_id',
  `app_name` varchar(128) COLLATE utf8mb3_bin DEFAULT NULL COMMENT 'app_name',
  `content` longtext COLLATE utf8mb3_bin NOT NULL COMMENT 'content',
  `md5` varchar(32) COLLATE utf8mb3_bin DEFAULT NULL COMMENT 'md5',
  `gmt_create` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `gmt_modified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '修改时间',
  `src_user` text COLLATE utf8mb3_bin COMMENT 'source user',
  `src_ip` varchar(50) COLLATE utf8mb3_bin DEFAULT NULL COMMENT 'source ip',
  `op_type` char(10) COLLATE utf8mb3_bin DEFAULT NULL COMMENT 'operation type',
  `tenant_id` varchar(128) COLLATE utf8mb3_bin DEFAULT '' COMMENT '租户字段',
  `encrypted_data_key` text COLLATE utf8mb3_bin NOT NULL COMMENT '密钥',
  PRIMARY KEY (`nid`),
  KEY `idx_gmt_create` (`gmt_create`),
  KEY `idx_gmt_modified` (`gmt_modified`),
  KEY `idx_did` (`data_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_bin COMMENT='多租户改造';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `his_config_info`
--

LOCK TABLES `his_config_info` WRITE;
/*!40000 ALTER TABLE `his_config_info` DISABLE KEYS */;
INSERT INTO `his_config_info` VALUES (0,1,'ServiceCityA-dev.yaml','DEFAULT_GROUP','','# MySQL数据库连接字符串\r\nConnectionStrings:\r\n  MedicalInsuranceDb: Server=127.0.0.1;Port=3306;Database=medical_insurance;Uid=root;Pwd=12345678;AllowPublicKeyRetrieval=True;SslMode=None\r\n\r\n# 可选：Nacos自身配置（程序读取本配置用）\r\nNacos:\r\n  ServerAddresses: 127.0.0.1:8848\r\n  Namespace: public\r\n  Group: DEFAULT_GROUP','4065fc1d739419f298724d7b9adbad04','2026-06-24 17:05:42','2026-06-24 17:05:42',NULL,'0:0:0:0:0:0:0:1','I','',''),(1,2,'ServiceCityA-dev.yaml','DEFAULT_GROUP','','# MySQL数据库连接字符串\r\nConnectionStrings:\r\n  MedicalInsuranceDb: Server=127.0.0.1;Port=3306;Database=medical_insurance;Uid=root;Pwd=12345678;AllowPublicKeyRetrieval=True;SslMode=None\r\n\r\n# 可选：Nacos自身配置（程序读取本配置用）\r\nNacos:\r\n  ServerAddresses: 127.0.0.1:8848\r\n  Namespace: public\r\n  Group: DEFAULT_GROUP','4065fc1d739419f298724d7b9adbad04','2026-06-24 17:11:40','2026-06-24 17:11:40',NULL,'0:0:0:0:0:0:0:1','U','',''),(0,3,'ServiceCityB-dev.yaml','DEFAULT_GROUP','','# 数据库连接串（Key必须是DefaultConnection，和代码对应）\nConnectionStrings:\n  DefaultConnection: Server=127.0.0.1;Port=3306;Database=medical_insurance;Uid=root;Pwd=12345678;AllowPublicKeyRetrieval=True;SslMode=None\n\n# 附加业务配置（程序里CityCode、ServiceName会读取）\nCityCode: CITYA\nServiceName: ServiceCityA','d51be44116b489cd452c227c135f5dfc','2026-06-24 19:09:44','2026-06-24 19:09:44',NULL,'0:0:0:0:0:0:0:1','I','',''),(0,4,'ServiceCityC-dev.yaml','DEFAULT_GROUP','','# 数据库连接串（Key必须是DefaultConnection，和代码对应）\nConnectionStrings:\n  DefaultConnection: Server=127.0.0.1;Port=3306;Database=medical_insurance;Uid=root;Pwd=12345678;AllowPublicKeyRetrieval=True;SslMode=None\n\n# 附加业务配置（程序里CityCode、ServiceName会读取）\nCityCode: CITYA\nServiceName: ServiceCityA','d51be44116b489cd452c227c135f5dfc','2026-06-24 19:09:57','2026-06-24 19:09:57',NULL,'0:0:0:0:0:0:0:1','I','',''),(0,5,'ServiceCityA-dev.yaml','DEFAULT_GROUP','','# 数据库连接串（Key必须是DefaultConnection，和代码对应）\nConnectionStrings:\n  DefaultConnection: Server=127.0.0.1;Port=3306;Database=medical_insurance;Uid=root;Pwd=12345678;AllowPublicKeyRetrieval=True;SslMode=None\n\n# 附加业务配置（程序里CityCode、ServiceName会读取）\nCityCode: CITYA\nServiceName: ServiceCityA','d51be44116b489cd452c227c135f5dfc','2026-06-24 19:25:30','2026-06-24 19:25:30',NULL,'0:0:0:0:0:0:0:1','I','dev',''),(0,6,'ServiceCityB-dev.yaml','DEFAULT_GROUP','','# 数据库连接串（Key必须是DefaultConnection，和代码对应）\nConnectionStrings:\n  DefaultConnection: Server=127.0.0.1;Port=3306;Database=medical_insurance;Uid=root;Pwd=12345678;AllowPublicKeyRetrieval=True;SslMode=None\n\n# 附加业务配置（程序里CityCode、ServiceName会读取）\nCityCode: CITYA\nServiceName: ServiceCityA','d51be44116b489cd452c227c135f5dfc','2026-06-24 19:25:38','2026-06-24 19:25:39',NULL,'0:0:0:0:0:0:0:1','I','dev',''),(0,7,'ServiceCityC-dev.yaml','DEFAULT_GROUP','','# 数据库连接串（Key必须是DefaultConnection，和代码对应）\nConnectionStrings:\n  DefaultConnection: Server=127.0.0.1;Port=3306;Database=medical_insurance;Uid=root;Pwd=12345678;AllowPublicKeyRetrieval=True;SslMode=None\n\n# 附加业务配置（程序里CityCode、ServiceName会读取）\nCityCode: CITYA\nServiceName: ServiceCityA','d51be44116b489cd452c227c135f5dfc','2026-06-24 19:25:38','2026-06-24 19:25:39',NULL,'0:0:0:0:0:0:0:1','I','dev',''),(1,8,'ServiceCityA-dev.yaml','DEFAULT_GROUP','','# 数据库连接串（Key必须是DefaultConnection，和代码对应）\nConnectionStrings:\n  DefaultConnection: Server=127.0.0.1;Port=3306;Database=medical_insurance;Uid=root;Pwd=12345678;AllowPublicKeyRetrieval=True;SslMode=None\n\n# 附加业务配置（程序里CityCode、ServiceName会读取）\nCityCode: CITYA\nServiceName: ServiceCityA','d51be44116b489cd452c227c135f5dfc','2026-06-24 19:25:46','2026-06-24 19:25:46',NULL,'0:0:0:0:0:0:0:1','D','',''),(2,9,'ServiceCityB-dev.yaml','DEFAULT_GROUP','','# 数据库连接串（Key必须是DefaultConnection，和代码对应）\nConnectionStrings:\n  DefaultConnection: Server=127.0.0.1;Port=3306;Database=medical_insurance;Uid=root;Pwd=12345678;AllowPublicKeyRetrieval=True;SslMode=None\n\n# 附加业务配置（程序里CityCode、ServiceName会读取）\nCityCode: CITYA\nServiceName: ServiceCityA','d51be44116b489cd452c227c135f5dfc','2026-06-24 19:25:46','2026-06-24 19:25:46',NULL,'0:0:0:0:0:0:0:1','D','',''),(3,10,'ServiceCityC-dev.yaml','DEFAULT_GROUP','','# 数据库连接串（Key必须是DefaultConnection，和代码对应）\nConnectionStrings:\n  DefaultConnection: Server=127.0.0.1;Port=3306;Database=medical_insurance;Uid=root;Pwd=12345678;AllowPublicKeyRetrieval=True;SslMode=None\n\n# 附加业务配置（程序里CityCode、ServiceName会读取）\nCityCode: CITYA\nServiceName: ServiceCityA','d51be44116b489cd452c227c135f5dfc','2026-06-24 19:25:46','2026-06-24 19:25:46',NULL,'0:0:0:0:0:0:0:1','D','','');
/*!40000 ALTER TABLE `his_config_info` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-06-24 20:23:33
