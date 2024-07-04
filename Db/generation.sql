--
-- File generated with SQLiteStudio v3.4.4 on Вс июн 30 20:49:36 2024
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: appointment
DROP TABLE IF EXISTS appointment;

CREATE TABLE IF NOT EXISTS appointment (
    app_id        INTEGER NOT NULL
                          CONSTRAINT PK_appointment PRIMARY KEY AUTOINCREMENT,
    app_date_time TEXT    NOT NULL,
    app_patient   INTEGER NOT NULL,
    CONSTRAINT FK_appointment_patient_app_patient FOREIGN KEY (
        app_patient
    )
    REFERENCES patient (pat_id) ON DELETE CASCADE
);


-- Table: map
DROP TABLE IF EXISTS map;

CREATE TABLE IF NOT EXISTS map (
    map_id                   INTEGER NOT NULL
                                     CONSTRAINT PK_map PRIMARY KEY AUTOINCREMENT,
    map_coordinates_json     TEXT    NOT NULL,
    map_created_at_date_time TEXT    NOT NULL,
    map_name                 TEXT    COLLATE NOCASE
                                     NOT NULL
);


-- Table: path_in_target
DROP TABLE IF EXISTS path_in_target;

CREATE TABLE IF NOT EXISTS path_in_target (
    pit_session          INTEGER NOT NULL,
    pit_target_id        INTEGER NOT NULL,
    pit_coordinates_json TEXT    NOT NULL,
    pit_precision        REAL    NOT NULL
                                 DEFAULT 0,
    CONSTRAINT PK_path_in_target PRIMARY KEY (
        pit_session,
        pit_target_id
    ),
    CONSTRAINT FK_path_in_target_session_pit_session FOREIGN KEY (
        pit_session
    )
    REFERENCES session (ses_id) ON DELETE RESTRICT
);


-- Table: path_to_target
DROP TABLE IF EXISTS path_to_target;

CREATE TABLE IF NOT EXISTS path_to_target (
    ptt_session          INTEGER NOT NULL,
    ptt_target_num       INTEGER NOT NULL,
    ptt_coordinates_json TEXT    NOT NULL,
    ptt_ange_distance    REAL    NOT NULL,
    ptt_angle_speed      REAL    NOT NULL,
    ptt_approach_speed   REAL    NOT NULL,
    ptt_time             REAL    NOT NULL,
    CONSTRAINT PK_path_to_target PRIMARY KEY (
        ptt_session,
        ptt_target_num
    ),
    CONSTRAINT FK_path_to_target_session_ptt_session FOREIGN KEY (
        ptt_session
    )
    REFERENCES session (ses_id) ON DELETE RESTRICT
);


-- Table: patient
DROP TABLE IF EXISTS patient;

CREATE TABLE IF NOT EXISTS patient (
    pat_id            INTEGER   NOT NULL
                                CONSTRAINT PK_patient PRIMARY KEY AUTOINCREMENT,
    pat_name          TEXT (20) COLLATE NOCASE
                                NOT NULL,
    pat_surname       TEXT (30) COLLATE NOCASE
                                NOT NULL,
    pat_patronymic    TEXT (30) COLLATE NOCASE,
    pat_date_of_birth TEXT      NOT NULL,
    pat_phone_mobile  TEXT      NOT NULL,
    pat_phone_home    TEXT
);


-- Table: session
DROP TABLE IF EXISTS session;

CREATE TABLE IF NOT EXISTS session (
    ses_id            INTEGER NOT NULL
                              CONSTRAINT PK_session PRIMARY KEY AUTOINCREMENT,
    ses_map           INTEGER NOT NULL,
    ses_max_x_angle   REAL    NOT NULL,
    ses_max_y_angle   REAL    NOT NULL,
    ses_log_file_path TEXT    NOT NULL,
    ses_date_time     TEXT    NOT NULL,
    ses_appointment   INTEGER NOT NULL,
    CONSTRAINT FK_session_appointment_ses_appointment FOREIGN KEY (
        ses_appointment
    )
    REFERENCES appointment (app_id),
    CONSTRAINT FK_session_map_ses_map FOREIGN KEY (
        ses_map
    )
    REFERENCES map (map_id) ON DELETE RESTRICT
);


-- Table: session_result
DROP TABLE IF EXISTS session_result;

CREATE TABLE IF NOT EXISTS session_result (
    sres_id         INTEGER NOT NULL
                            CONSTRAINT PK_session_result PRIMARY KEY,
    sres_math_exp   REAL    NOT NULL,
    sres_deviation  REAL    NOT NULL,
    sres_dispersion REAL    NOT NULL,
    sres_score      INTEGER NOT NULL,
    CONSTRAINT FK_session_result_session_sres_id FOREIGN KEY (
        sres_id
    )
    REFERENCES session (ses_id) ON DELETE RESTRICT
);


-- Index: IX_appointment_app_patient
DROP INDEX IF EXISTS IX_appointment_app_patient;

CREATE INDEX IF NOT EXISTS IX_appointment_app_patient ON appointment (
    "app_patient"
);


-- Index: IX_map_map_name
DROP INDEX IF EXISTS IX_map_map_name;

CREATE UNIQUE INDEX IF NOT EXISTS IX_map_map_name ON map (
    "map_name"
);


-- Index: IX_session_ses_appointment
DROP INDEX IF EXISTS IX_session_ses_appointment;

CREATE INDEX IF NOT EXISTS IX_session_ses_appointment ON session (
    "ses_appointment"
);


-- Index: IX_session_ses_log_file_path
DROP INDEX IF EXISTS IX_session_ses_log_file_path;

CREATE UNIQUE INDEX IF NOT EXISTS IX_session_ses_log_file_path ON session (
    "ses_log_file_path"
);


-- Index: IX_session_ses_map
DROP INDEX IF EXISTS IX_session_ses_map;

CREATE INDEX IF NOT EXISTS IX_session_ses_map ON session (
    "ses_map"
);


COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
