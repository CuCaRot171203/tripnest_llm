-- ======================================================
-- MyTrip / Tripnest full DDL (MySQL 8+)
-- - Creates DB tripnest with utf8mb4_vietnamese_ci
-- - Creates tables IF NOT EXISTS
-- - Creates indexes conditionally (avoids "index exists" errors)
-- ======================================================

-- Create database and use it
CREATE DATABASE IF NOT EXISTS `tripnest`
  CHARACTER SET = utf8mb4
  COLLATE = utf8mb4_vietnamese_ci;
USE `tripnest`;

-- -------------------------
-- Tables (IF NOT EXISTS)
-- -------------------------

CREATE TABLE IF NOT EXISTS `Roles` (
  `role_id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  `description` VARCHAR(255),
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`role_id`),
  UNIQUE KEY `uq_roles_name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `Users` (
  `user_id` CHAR(36) NOT NULL,
  `role_id` INT DEFAULT NULL,
  `email` VARCHAR(255) NOT NULL,
  `password_hash` VARCHAR(512),
  `full_name` VARCHAR(255),
  `phone` VARCHAR(50),
  `locale` VARCHAR(8) DEFAULT 'vi',
  `profile_photo_url` VARCHAR(1024),
  `is_active` TINYINT(1) DEFAULT 1,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `uq_users_email` (`email`),
  KEY `idx_users_role` (`role_id`),
  CONSTRAINT `fk_users_roles` FOREIGN KEY (`role_id`) REFERENCES `Roles` (`role_id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `RefreshTokens` (
  `token_id` CHAR(36) NOT NULL,
  `user_id` CHAR(36) NOT NULL,
  `token_hash` VARCHAR(512) NOT NULL,
  `device_info` VARCHAR(255),
  `expires_at` DATETIME,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `revoked_at` DATETIME DEFAULT NULL,
  PRIMARY KEY (`token_id`),
  KEY `idx_rt_user` (`user_id`),
  CONSTRAINT `fk_rt_user` FOREIGN KEY (`user_id`) REFERENCES `Users` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `Companies` (
  `company_id` CHAR(36) NOT NULL,
  `name` VARCHAR(255) NOT NULL,
  `slug` VARCHAR(255) DEFAULT NULL,
  `address` VARCHAR(512) DEFAULT NULL,
  `phone` VARCHAR(50) DEFAULT NULL,
  `email` VARCHAR(255) DEFAULT NULL,
  `owner_user_id` CHAR(36) DEFAULT NULL,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`company_id`),
  UNIQUE KEY `uq_companies_slug` (`slug`),
  KEY `idx_companies_owner` (`owner_user_id`),
  CONSTRAINT `fk_companies_owner` FOREIGN KEY (`owner_user_id`) REFERENCES `Users` (`user_id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `CompanyRoles` (
  `company_role_id` INT NOT NULL AUTO_INCREMENT,
  `company_id` CHAR(36) NOT NULL,
  `name` VARCHAR(100) NOT NULL,
  `description` VARCHAR(255),
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`company_role_id`),
  UNIQUE KEY `uq_companyrole_company_name` (`company_id`, `name`),
  KEY `idx_companyrole_company` (`company_id`),
  CONSTRAINT `fk_companyrole_company` FOREIGN KEY (`company_id`) REFERENCES `Companies` (`company_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `CompanyEmployees` (
  `company_employee_id` CHAR(36) NOT NULL,
  `company_id` CHAR(36) NOT NULL,
  `user_id` CHAR(36) NOT NULL,
  `company_role_id` INT DEFAULT NULL,
  `title` VARCHAR(100) DEFAULT NULL,
  `is_active` TINYINT(1) DEFAULT 1,
  `joined_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `left_at` DATETIME DEFAULT NULL,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`company_employee_id`),
  UNIQUE KEY `uq_company_user` (`company_id`, `user_id`),
  KEY `idx_companyemployees_company` (`company_id`),
  KEY `idx_companyemployees_user` (`user_id`),
  CONSTRAINT `fk_companyemployees_company` FOREIGN KEY (`company_id`) REFERENCES `Companies` (`company_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_companyemployees_user` FOREIGN KEY (`user_id`) REFERENCES `Users` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_companyemployees_companyrole` FOREIGN KEY (`company_role_id`) REFERENCES `CompanyRoles` (`company_role_id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `CompanyPermissions` (
  `permission_id` INT NOT NULL AUTO_INCREMENT,
  `company_id` CHAR(36) NOT NULL,
  `name` VARCHAR(150) NOT NULL,
  `description` VARCHAR(255),
  PRIMARY KEY (`permission_id`),
  UNIQUE KEY `uq_companypermission_company_name` (`company_id`, `name`),
  KEY `idx_companypermissions_company` (`company_id`),
  CONSTRAINT `fk_companypermissions_company` FOREIGN KEY (`company_id`) REFERENCES `Companies` (`company_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `CompanyRolePermissions` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `company_role_id` INT NOT NULL,
  `permission_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_crp` (`company_role_id`, `permission_id`),
  KEY `idx_crp_role` (`company_role_id`),
  KEY `idx_crp_perm` (`permission_id`),
  CONSTRAINT `fk_crp_role` FOREIGN KEY (`company_role_id`) REFERENCES `CompanyRoles` (`company_role_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_crp_permission` FOREIGN KEY (`permission_id`) REFERENCES `CompanyPermissions` (`permission_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `Properties` (
  `property_id` BIGINT NOT NULL AUTO_INCREMENT,
  `owner_user_id` CHAR(36) DEFAULT NULL,
  `company_id` CHAR(36) DEFAULT NULL,
  `title_vi` VARCHAR(255) DEFAULT NULL,
  `title_en` VARCHAR(255) DEFAULT NULL,
  `description_vi` TEXT,
  `description_en` TEXT,
  `street` VARCHAR(255),
  `city` VARCHAR(100),
  `province` VARCHAR(100),
  `country` VARCHAR(100),
  `latitude` DOUBLE,
  `longitude` DOUBLE,
  `address_formatted` VARCHAR(512),
  `property_type` VARCHAR(50),
  `price_base` DECIMAL(12,2),
  `currency` CHAR(3) DEFAULT 'USD',
  `status` TINYINT(1) DEFAULT 1,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `location` POINT NOT NULL,
  PRIMARY KEY (`property_id`),
  KEY `idx_properties_owner` (`owner_user_id`),
  KEY `idx_properties_company` (`company_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `PropertyPhotos` (
  `photo_id` BIGINT NOT NULL AUTO_INCREMENT,
  `property_id` BIGINT NOT NULL,
  `url` VARCHAR(1024) NOT NULL,
  `order` INT DEFAULT 0,
  `meta` JSON,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`photo_id`),
  KEY `idx_photos_property` (`property_id`),
  CONSTRAINT `fk_photos_property` FOREIGN KEY (`property_id`) REFERENCES `Properties` (`property_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `Amenities` (
  `amenity_id` INT NOT NULL AUTO_INCREMENT,
  `name_vi` VARCHAR(100) NOT NULL,
  `name_en` VARCHAR(100) DEFAULT NULL,
  `slug` VARCHAR(100) DEFAULT NULL,
  PRIMARY KEY (`amenity_id`),
  UNIQUE KEY `uq_amenities_name_vi` (`name_vi`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `PropertyAmenities` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `property_id` BIGINT NOT NULL,
  `amenity_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_property_amenity` (`property_id`, `amenity_id`),
  KEY `idx_pa_property` (`property_id`),
  KEY `idx_pa_amenity` (`amenity_id`),
  CONSTRAINT `fk_pa_property` FOREIGN KEY (`property_id`) REFERENCES `Properties` (`property_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_pa_amenity` FOREIGN KEY (`amenity_id`) REFERENCES `Amenities` (`amenity_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `Rooms` (
  `room_id` BIGINT NOT NULL AUTO_INCREMENT,
  `property_id` BIGINT NOT NULL,
  `name_vi` VARCHAR(255) DEFAULT NULL,
  `name_en` VARCHAR(255) DEFAULT NULL,
  `capacity` INT DEFAULT 1,
  `price_per_night` DECIMAL(12,2),
  `stock` INT DEFAULT 1,
  `cancellation_policy` JSON,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`room_id`),
  KEY `idx_rooms_property` (`property_id`),
  CONSTRAINT `fk_rooms_property` FOREIGN KEY (`property_id`) REFERENCES `Properties` (`property_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `RoomAvailability` (
  `availability_id` BIGINT NOT NULL AUTO_INCREMENT,
  `room_id` BIGINT NOT NULL,
  `date` DATE NOT NULL,
  `available_count` INT NOT NULL DEFAULT 0,
  PRIMARY KEY (`availability_id`),
  UNIQUE KEY `uq_room_date` (`room_id`, `date`),
  KEY `idx_roomavailability_room_date` (`room_id`, `date`),
  CONSTRAINT `fk_roomavailability_room` FOREIGN KEY (`room_id`) REFERENCES `Rooms` (`room_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `RoomPrices` (
  `room_price_id` BIGINT NOT NULL AUTO_INCREMENT,
  `room_id` BIGINT NOT NULL,
  `valid_from` DATE NOT NULL,
  `valid_to` DATE NOT NULL,
  `price` DECIMAL(12,2) NOT NULL,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`room_price_id`),
  KEY `idx_roomprices_room` (`room_id`),
  CONSTRAINT `fk_roomprices_room` FOREIGN KEY (`room_id`) REFERENCES `Rooms` (`room_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `Bookings` (
  `booking_id` CHAR(36) NOT NULL,
  `user_id` CHAR(36) NOT NULL,
  `property_id` BIGINT NOT NULL,
  `status` ENUM('Pending','Confirmed','Cancelled','Completed') DEFAULT 'Pending',
  `checkin_date` DATE NOT NULL,
  `checkout_date` DATE NOT NULL,
  `guests_count` INT DEFAULT 1,
  `total_price` DECIMAL(12,2),
  `currency` CHAR(3) DEFAULT 'USD',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `version` INT DEFAULT 1,
  PRIMARY KEY (`booking_id`),
  KEY `idx_bookings_user` (`user_id`),
  KEY `idx_bookings_property_status` (`property_id`, `status`),
  CONSTRAINT `fk_bookings_user` FOREIGN KEY (`user_id`) REFERENCES `Users` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_bookings_property` FOREIGN KEY (`property_id`) REFERENCES `Properties` (`property_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `BookingItems` (
  `booking_item_id` BIGINT NOT NULL AUTO_INCREMENT,
  `booking_id` CHAR(36) NOT NULL,
  `room_id` BIGINT NOT NULL,
  `price` DECIMAL(12,2),
  `nights` INT,
  `qty` INT DEFAULT 1,
  `subtotal` DECIMAL(12,2),
  PRIMARY KEY (`booking_item_id`),
  KEY `idx_bookingitems_booking` (`booking_id`),
  KEY `idx_bookingitems_room` (`room_id`),
  CONSTRAINT `fk_bookingitems_booking` FOREIGN KEY (`booking_id`) REFERENCES `Bookings` (`booking_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_bookingitems_room` FOREIGN KEY (`room_id`) REFERENCES `Rooms` (`room_id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `Payments` (
  `payment_id` CHAR(36) NOT NULL,
  `booking_id` CHAR(36) DEFAULT NULL,
  `provider` VARCHAR(100),
  `provider_ref` VARCHAR(255),
  `amount` DECIMAL(12,2),
  `currency` CHAR(3),
  `status` VARCHAR(50),
  `paid_at` DATETIME DEFAULT NULL,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`payment_id`),
  KEY `idx_payments_booking` (`booking_id`),
  KEY `idx_payments_provider_ref` (`provider_ref`),
  CONSTRAINT `fk_payments_booking` FOREIGN KEY (`booking_id`) REFERENCES `Bookings` (`booking_id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `Reviews` (
  `review_id` CHAR(36) NOT NULL,
  `property_id` BIGINT NOT NULL,
  `user_id` CHAR(36) NOT NULL,
  `rating` TINYINT NOT NULL,
  `title` VARCHAR(255),
  `content` TEXT,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`review_id`),
  KEY `idx_reviews_property` (`property_id`),
  KEY `idx_reviews_user` (`user_id`),
  CONSTRAINT `fk_reviews_property` FOREIGN KEY (`property_id`) REFERENCES `Properties` (`property_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_reviews_user` FOREIGN KEY (`user_id`) REFERENCES `Users` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `Messages` (
  `message_id` CHAR(36) NOT NULL,
  `from_user_id` CHAR(36) DEFAULT NULL,
  `to_user_id` CHAR(36) DEFAULT NULL,
  `property_id` BIGINT DEFAULT NULL,
  `content` TEXT,
  `is_ai` TINYINT(1) DEFAULT 0,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`message_id`),
  KEY `idx_messages_from` (`from_user_id`),
  KEY `idx_messages_to` (`to_user_id`),
  KEY `idx_messages_property` (`property_id`),
  CONSTRAINT `fk_messages_from` FOREIGN KEY (`from_user_id`) REFERENCES `Users` (`user_id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_messages_to` FOREIGN KEY (`to_user_id`) REFERENCES `Users` (`user_id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_messages_property` FOREIGN KEY (`property_id`) REFERENCES `Properties` (`property_id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `Itineraries` (
  `itinerary_id` CHAR(36) NOT NULL,
  `user_id` CHAR(36) NOT NULL,
  `name_vi` VARCHAR(255) DEFAULT NULL,
  `name_en` VARCHAR(255) DEFAULT NULL,
  `start_date` DATE,
  `end_date` DATE,
  `metadata` JSON,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`itinerary_id`),
  KEY `idx_itineraries_user` (`user_id`),
  CONSTRAINT `fk_itineraries_user` FOREIGN KEY (`user_id`) REFERENCES `Users` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `Waypoints` (
  `waypoint_id` BIGINT NOT NULL AUTO_INCREMENT,
  `itinerary_id` CHAR(36) NOT NULL,
  `order` INT DEFAULT 0,
  `lat` DOUBLE,
  `lng` DOUBLE,
  `place_id` VARCHAR(255),
  `arrival` DATETIME DEFAULT NULL,
  PRIMARY KEY (`waypoint_id`),
  KEY `idx_waypoints_itinerary` (`itinerary_id`),
  CONSTRAINT `fk_waypoints_itinerary` FOREIGN KEY (`itinerary_id`) REFERENCES `Itineraries` (`itinerary_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `Embeddings` (
  `embedding_id` BIGINT NOT NULL AUTO_INCREMENT,
  `item_type` VARCHAR(50) NOT NULL,
  `item_id` VARCHAR(255) NOT NULL,
  `vector_ref` VARCHAR(255),
  `vector_blob` LONGBLOB,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`embedding_id`),
  KEY `idx_embeddings_item` (`item_type`, `item_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `AuditLogs` (
  `audit_id` CHAR(36) NOT NULL,
  `entity_type` VARCHAR(100),
  `entity_id` VARCHAR(255),
  `action` VARCHAR(100),
  `before_json` JSON,
  `after_json` JSON,
  `performed_by` CHAR(36),
  `performed_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`audit_id`),
  KEY `idx_audit_performed_by` (`performed_by`),
  CONSTRAINT `fk_audit_user` FOREIGN KEY (`performed_by`) REFERENCES `Users` (`user_id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;

CREATE TABLE IF NOT EXISTS `Notifications` (
  `notification_id` CHAR(36) NOT NULL,
  `user_id` CHAR(36) NOT NULL,
  `type` VARCHAR(100),
  `payload` JSON,
  `is_read` TINYINT(1) DEFAULT 0,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`notification_id`),
  KEY `idx_notifications_user` (`user_id`),
  CONSTRAINT `fk_notifications_user` FOREIGN KEY (`user_id`) REFERENCES `Users` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_vietnamese_ci;


-- -------------------------
-- Conditional index creation (check information_schema)
-- -------------------------

-- Helper variables
SET @schema_name = 'tripnest';

-- 1) Fulltext index: title_vi + description_vi
SET @table_name = 'Properties';
SET @index_name = 'ft_properties_title_description_vi';
SELECT COUNT(*) INTO @exists_idx
  FROM information_schema.statistics
  WHERE table_schema = @schema_name
    AND table_name = @table_name
    AND index_name = @index_name;

SET @sql_stmt = IF(@exists_idx = 0,
  'ALTER TABLE `tripnest`.`Properties` ADD FULLTEXT INDEX `ft_properties_title_description_vi` (`title_vi`, `description_vi`);',
  'SELECT \"ft_properties_title_description_vi already exists\";'
);
PREPARE stmt FROM @sql_stmt; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 2) Fulltext index: title_en + description_en
SET @index_name = 'ft_properties_title_description_en';
SELECT COUNT(*) INTO @exists_idx
  FROM information_schema.statistics
  WHERE table_schema = @schema_name
    AND table_name = @table_name
    AND index_name = @index_name;

SET @sql_stmt = IF(@exists_idx = 0,
  'ALTER TABLE `tripnest`.`Properties` ADD FULLTEXT INDEX `ft_properties_title_description_en` (`title_en`, `description_en`);',
  'SELECT \"ft_properties_title_description_en already exists\";'
);
PREPARE stmt FROM @sql_stmt; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 3) Spatial index on location (requires NOT NULL)
SET @index_name = 'sp_idx_properties_location';
SELECT COUNT(*) INTO @exists_idx
  FROM information_schema.statistics
  WHERE table_schema = @schema_name
    AND table_name = @table_name
    AND index_name = @index_name;

SET @sql_stmt = IF(@exists_idx = 0,
  'CREATE SPATIAL INDEX `sp_idx_properties_location` ON `tripnest`.`Properties` (`location`);',
  'SELECT \"sp_idx_properties_location already exists\";'
);
PREPARE stmt FROM @sql_stmt; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 4) BTREE index: price_base
SET @index_name = 'idx_properties_price';
SELECT COUNT(*) INTO @exists_idx
  FROM information_schema.statistics
  WHERE table_schema = @schema_name
    AND table_name = @table_name
    AND index_name = @index_name;

SET @sql_stmt = IF(@exists_idx = 0,
  'ALTER TABLE `tripnest`.`Properties` ADD INDEX `idx_properties_price` (`price_base`);',
  'SELECT \"idx_properties_price already exists\";'
);
PREPARE stmt FROM @sql_stmt; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 5) Index on Reviews.rating
SET @table_name = 'Reviews';
SET @index_name = 'idx_reviews_rating';
SELECT COUNT(*) INTO @exists_idx
  FROM information_schema.statistics
  WHERE table_schema = @schema_name
    AND table_name = @table_name
    AND index_name = @index_name;

SET @sql_stmt = IF(@exists_idx = 0,
  'ALTER TABLE `tripnest`.`Reviews` ADD INDEX `idx_reviews_rating` (`rating`);',
  'SELECT \"idx_reviews_rating already exists\";'
);
PREPARE stmt FROM @sql_stmt; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- 6) Optional: index for common lookup (Properties owner, company already created as KEYs in CREATE TABLE)
-- 7) Any other indexes already declared in CREATE TABLE will exist (KEY ...).
-- End of script.users

-- ======================================================
-- Usage notes:
-- - Insert Properties with location:
--     INSERT INTO Properties (owner_user_id, title_vi, title_en, description_vi, description_en, latitude, longitude, location)
--     VALUES ('uuid-user', 'Tiêu đề VN', 'Title EN', 'Mô tả VN', 'Description EN', 10.762622, 106.660172, ST_PointFromText('POINT(106.660172 10.762622)'));
--   or if ST_Point is available: ST_Point(106.660172, 10.762622)
-- - Backend: choose title_vi/description_vi when Accept-Language = 'vi', fallback to EN.
-- ======================================================
