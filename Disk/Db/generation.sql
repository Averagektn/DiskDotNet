--
-- File generated with SQLiteStudio v3.4.4 on вт апр. 30 23:23:11 2024
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: address
DROP TABLE IF EXISTS address;

CREATE TABLE IF NOT EXISTS address (
    addr_id        INTEGER PRIMARY KEY AUTOINCREMENT,
    addr_region    INTEGER NOT NULL
                           REFERENCES region (rgn_id) ON DELETE RESTRICT
                                                      ON UPDATE CASCADE,
    addr_street    TEXT    NOT NULL
                           COLLATE NOCASE
                           COLLATE RTRIM,
    addr_house     INTEGER NOT NULL
                           CONSTRAINT is_positive CHECK (addr_street > 0),
    addr_apartment INTEGER NOT NULL
                           CONSTRAINT is_postitive CHECK (addr_apartment > 0),
    addr_corpus    INTEGER DEFAULT (1) 
                           NOT NULL
                           CONSTRAINT is_postitive CHECK (addr_corpus > 0) 
);


-- Table: appointment
DROP TABLE IF EXISTS appointment;

CREATE TABLE IF NOT EXISTS appointment (
    app_id      INTEGER PRIMARY KEY AUTOINCREMENT,
    app_time    TEXT    NOT NULL
                        COLLATE NOCASE
                        COLLATE RTRIM,
    app_doctor  INTEGER REFERENCES doctor (doc_id) ON DELETE RESTRICT
                                                   ON UPDATE CASCADE
                        NOT NULL,
    app_patient INTEGER REFERENCES patient (pat_id) ON DELETE CASCADE
                                                    ON UPDATE CASCADE
                        NOT NULL
);


-- Table: card
DROP TABLE IF EXISTS card;

CREATE TABLE IF NOT EXISTS card (
    crd_id      INTEGER  PRIMARY KEY AUTOINCREMENT,
    crd_patient INTEGER  REFERENCES patient (pat_id) ON DELETE CASCADE
                                                     ON UPDATE CASCADE
                         NOT NULL,
    crd_number  TEXT (9) NOT NULL
                         COLLATE NOCASE
                         COLLATE RTRIM
);


-- Table: contraindication
DROP TABLE IF EXISTS contraindication;

CREATE TABLE IF NOT EXISTS contraindication (
    con_id   INTEGER PRIMARY KEY AUTOINCREMENT,
    con_card INTEGER REFERENCES card (crd_id) ON DELETE CASCADE
                                              ON UPDATE CASCADE
                     NOT NULL,
    con_name TEXT    NOT NULL
                     COLLATE NOCASE
                     COLLATE RTRIM
);


-- Table: diagnosis
DROP TABLE IF EXISTS diagnosis;

CREATE TABLE IF NOT EXISTS diagnosis (
    dia_id   INTEGER PRIMARY KEY AUTOINCREMENT,
    dia_name TEXT    UNIQUE
                     NOT NULL
                     COLLATE NOCASE
                     COLLATE RTRIM
);


-- Table: district
DROP TABLE IF EXISTS district;

CREATE TABLE IF NOT EXISTS district (
    dst_id     INTEGER PRIMARY KEY AUTOINCREMENT,
    dst_name   TEXT    UNIQUE
                       NOT NULL
                       COLLATE NOCASE
                       COLLATE RTRIM,
    dst_region INTEGER NOT NULL
                       REFERENCES region (rgn_id) ON DELETE RESTRICT
                                                  ON UPDATE CASCADE
);


-- Table: doctor
DROP TABLE IF EXISTS doctor;

CREATE TABLE IF NOT EXISTS doctor (
    doc_id         INTEGER PRIMARY KEY AUTOINCREMENT,
    doc_name       TEXT    NOT NULL
                           COLLATE NOCASE
                           COLLATE RTRIM,
    doc_surname    TEXT    NOT NULL
                           COLLATE NOCASE
                           COLLATE RTRIM,
    doc_patronymic TEXT    COLLATE NOCASE
                           COLLATE RTRIM,
    doc_password   TEXT    NOT NULL
                           COLLATE NOCASE
                           COLLATE RTRIM
);


-- Table: doctor_cabinet
DROP TABLE IF EXISTS doctor_cabinet;

CREATE TABLE IF NOT EXISTS doctor_cabinet (
    dc_id          INTEGER PRIMARY KEY AUTOINCREMENT,
    dc_floor       INTEGER NOT NULL
                           CHECK (dc_floor > 0),
    dc_cabinet_num INTEGER NOT NULL
                           CHECK (dc_cabinet_num > 0),
    dc_name        TEXT    NOT NULL
                           COLLATE NOCASE
                           COLLATE RTRIM,
    UNIQUE (
        dc_floor,
        dc_cabinet_num
    )
);


-- Table: m2m_card_diagnosis
DROP TABLE IF EXISTS m2m_card_diagnosis;

CREATE TABLE IF NOT EXISTS m2m_card_diagnosis (
    c2d_card             INTEGER REFERENCES card (crd_id) ON DELETE CASCADE
                                                          ON UPDATE CASCADE,
    c2d_diagnosis        INTEGER REFERENCES diagnosis (dia_id) ON DELETE RESTRICT
                                                               ON UPDATE CASCADE,
    c2d_diagnosis_start  TEXT    NOT NULL
                                 COLLATE NOCASE
                                 COLLATE RTRIM,
    c2d_diagnosis_finish TEXT    COLLATE NOCASE
                                 COLLATE RTRIM,
    PRIMARY KEY (
        c2d_card,
        c2d_diagnosis
    )
);


-- Table: map
DROP TABLE IF EXISTS map;

CREATE TABLE IF NOT EXISTS map (
    map_id               INTEGER PRIMARY KEY AUTOINCREMENT,
    map_coordinates_json TEXT    NOT NULL,
    map_created_at       TEXT    NOT NULL
                                 COLLATE NOCASE
                                 COLLATE RTRIM,
    map_created_by       INTEGER REFERENCES doctor (doc_id) ON DELETE RESTRICT
                                                            ON UPDATE CASCADE
                                 NOT NULL,
    map_name             TEXT    UNIQUE
                                 NOT NULL
                                 COLLATE NOCASE
                                 COLLATE RTRIM
);


-- Table: note
DROP TABLE IF EXISTS note;

CREATE TABLE IF NOT EXISTS note (
    nt_id      INTEGER PRIMARY KEY AUTOINCREMENT,
    nt_patient INTEGER REFERENCES patient (pat_id) ON DELETE CASCADE
                                                   ON UPDATE CASCADE
                       NOT NULL,
    nt_doctor  INTEGER REFERENCES doctor (doc_id) ON DELETE RESTRICT
                                                  ON UPDATE CASCADE
                       NOT NULL,
    nt_text    TEXT    NOT NULL
);


-- Table: operation
DROP TABLE IF EXISTS operation;

CREATE TABLE IF NOT EXISTS operation (
    op_id          INTEGER PRIMARY KEY AUTOINCREMENT,
    op_card        INTEGER REFERENCES card (crd_id) ON DELETE CASCADE
                                                    ON UPDATE CASCADE
                           NOT NULL,
    op_asingned_by INTEGER REFERENCES doctor (doc_id) ON DELETE RESTRICT
                                                      ON UPDATE CASCADE,
    op_name        TEXT    NOT NULL
                           COLLATE NOCASE
                           COLLATE RTRIM,
    op_cabinet     INTEGER REFERENCES doctor_cabinet (dc_id) ON DELETE RESTRICT
                                                             ON UPDATE CASCADE,
    op_date_time   TEXT    NOT NULL
                           COLLATE RTRIM
                           COLLATE NOCASE
);


-- Table: path_in_target
DROP TABLE IF EXISTS path_in_target;

CREATE TABLE IF NOT EXISTS path_in_target (
    pit_session          INTEGER REFERENCES session (ses_id) ON DELETE CASCADE
                                                             ON UPDATE CASCADE,
    pit_target_id        INTEGER,
    pit_coordinates_json TEXT    NOT NULL,
    PRIMARY KEY (
        pit_session,
        pit_target_id
    )
);


-- Table: path_to_target
DROP TABLE IF EXISTS path_to_target;

CREATE TABLE IF NOT EXISTS path_to_target (
    ptt_session          INTEGER REFERENCES session (ses_id) ON DELETE CASCADE
                                                             ON UPDATE CASCADE,
    ptt_num              INTEGER,
    ptt_coordinates_json TEXT    NOT NULL,
    ptt_time             REAL    NOT NULL,
    ptt_angle_distance   REAL    NOT NULL,
    ptt_angle_speed      REAL    NOT NULL,
    ptt_approach_speed   REAL    NOT NULL,
    PRIMARY KEY (
        ptt_session,
        ptt_num
    )
);


-- Table: patient
DROP TABLE IF EXISTS patient;

CREATE TABLE IF NOT EXISTS patient (
    pat_id            INTEGER   PRIMARY KEY AUTOINCREMENT,
    pat_name          TEXT (20) NOT NULL
                                COLLATE RTRIM
                                COLLATE NOCASE,
    pat_surname       TEXT (30) NOT NULL
                                COLLATE RTRIM
                                COLLATE NOCASE,
    pat_patronymic    TEXT (30) COLLATE RTRIM
                                COLLATE NOCASE,
    pat_address       INTEGER   REFERENCES address (addr_id) ON DELETE CASCADE
                                                             ON UPDATE CASCADE
                                NOT NULL,
    pat_date_of_birth TEXT      NOT NULL
                                COLLATE RTRIM
                                COLLATE NOCASE,
    pat_phone_mobile  TEXT      COLLATE RTRIM
                                COLLATE NOCASE,
    pat_phone_home    TEXT      COLLATE RTRIM
                                COLLATE NOCASE
);


-- Table: procedure
DROP TABLE IF EXISTS procedure;

CREATE TABLE IF NOT EXISTS procedure (
    pro_id          INTEGER PRIMARY KEY AUTOINCREMENT,
    pro_assigned_by INTEGER REFERENCES doctor (doc_id) ON DELETE RESTRICT
                                                       ON UPDATE CASCADE
                            NOT NULL,
    pro_assigned_to INTEGER REFERENCES patient (pat_id) ON DELETE CASCADE
                                                        ON UPDATE CASCADE
                            NOT NULL,
    pro_date_time   TEXT    NOT NULL
                            COLLATE RTRIM
                            COLLATE NOCASE,
    pro_name        TEXT    NOT NULL
                            COLLATE RTRIM
                            COLLATE NOCASE,
    pro_cabinet     INTEGER REFERENCES doctor_cabinet (dc_id) ON DELETE RESTRICT
                                                              ON UPDATE CASCADE
);


-- Table: region
DROP TABLE IF EXISTS region;

CREATE TABLE IF NOT EXISTS region (
    rgn_id   INTEGER PRIMARY KEY AUTOINCREMENT,
    rgn_name TEXT    UNIQUE
                     NOT NULL
                     COLLATE RTRIM
                     COLLATE NOCASE
);


-- Table: session
DROP TABLE IF EXISTS session;

CREATE TABLE IF NOT EXISTS session (
    ses_id            INTEGER PRIMARY KEY AUTOINCREMENT,
    ses_map           INTEGER REFERENCES map (map_id) ON DELETE RESTRICT
                                                      ON UPDATE CASCADE
                              NOT NULL,
    ses_log_file_path TEXT    NOT NULL
                              UNIQUE
                              COLLATE RTRIM,
    ses_date          TEXT    NOT NULL
                              COLLATE RTRIM
                              COLLATE NOCASE,
    ses_appointment   INTEGER NOT NULL
                              REFERENCES appointment (app_id) ON DELETE CASCADE
                                                              ON UPDATE CASCADE
);


-- Table: session_result
DROP TABLE IF EXISTS session_result;

CREATE TABLE IF NOT EXISTS session_result (
    sres_id         INTEGER PRIMARY KEY
                            REFERENCES session (ses_id) ON DELETE CASCADE
                                                        ON UPDATE CASCADE,
    sres_math_exp   REAL    NOT NULL,
    sres_deviation  REAL    NOT NULL,
    sres_dispersion REAL    NOT NULL,
    sres_score      INTEGER NOT NULL
                            CHECK (sres_score > 0) 
);


-- Table: target_file
DROP TABLE IF EXISTS target_file;

CREATE TABLE IF NOT EXISTS target_file (
    tf_id       INTEGER PRIMARY KEY AUTOINCREMENT,
    tf_filepath TEXT    NOT NULL
                        UNIQUE
                        COLLATE RTRIM,
    tf_added_by         REFERENCES doctor (doc_id) ON DELETE RESTRICT
                                                   ON UPDATE CASCADE
                        NOT NULL
);


-- Table: xray
DROP TABLE IF EXISTS xray;

CREATE TABLE IF NOT EXISTS xray (
    xr_id          INTEGER PRIMARY KEY AUTOINCREMENT,
    xr_date        TEXT    NOT NULL
                           COLLATE RTRIM
                           COLLATE NOCASE,
    xr_file_path   TEXT    UNIQUE
                           NOT NULL
                           COLLATE RTRIM,
    xr_description TEXT,
    xr_card        INTEGER REFERENCES card (crd_id) ON DELETE CASCADE
                                                    ON UPDATE CASCADE
                           NOT NULL
);


-- Index: IDX_name
DROP INDEX IF EXISTS IDX_name;

CREATE INDEX IF NOT EXISTS IDX_name ON patient (
    pat_name,
    pat_surname,
    pat_patronymic
);


-- Index: IDX_number
DROP INDEX IF EXISTS IDX_number;

CREATE INDEX IF NOT EXISTS IDX_number ON card (
    crd_number
);


-- Index: IDX_time
DROP INDEX IF EXISTS IDX_time;

CREATE INDEX IF NOT EXISTS IDX_time ON appointment (
    app_time DESC
);


-- Index: UNQ_IDX_number
DROP INDEX IF EXISTS UNQ_IDX_number;

CREATE UNIQUE INDEX IF NOT EXISTS UNQ_IDX_number ON card (
    crd_number
);


-- View: card_diagnosis
DROP VIEW IF EXISTS card_diagnosis;
CREATE VIEW IF NOT EXISTS card_diagnosis AS
    SELECT c.crd_id,
           c.crd_number,
           d.dia_id,
           d.dia_name,
           m.c2d_diagnosis_start,
           m.c2d_diagnosis_finish
      FROM card c
           INNER JOIN
           m2m_card_diagnosis m ON c.crd_id = m.c2d_card
           INNER JOIN
           diagnosis d ON m.c2d_diagnosis = d.dia_id;


-- View: doctor_appointment
DROP VIEW IF EXISTS doctor_appointment;
CREATE VIEW IF NOT EXISTS doctor_appointment AS
    SELECT d.doc_id,
           d.doc_name,
           d.doc_surname,
           d.doc_patronymic,
           a.app_id,
           a.app_time,
           p.pat_id,
           p.pat_name,
           p.pat_surname,
           p.pat_patronymic
      FROM doctor d
           INNER JOIN
           appointment a ON d.doc_id = a.app_doctor
           INNER JOIN
           patient p ON a.app_patient = p.pat_id;


-- View: patient_card
DROP VIEW IF EXISTS patient_card;
CREATE VIEW IF NOT EXISTS patient_card AS
    SELECT p.pat_id,
           p.pat_name,
           p.pat_surname,
           p.pat_patronymic,
           c.crd_id,
           c.crd_number
      FROM patient p
           INNER JOIN
           card c ON p.pat_id = c.crd_patient;


-- View: report
DROP VIEW IF EXISTS report;
CREATE VIEW IF NOT EXISTS report AS
    SELECT sr.sres_id,
           sr.sres_math_exp,
           sr.sres_deviation,
           sr.sres_dispersion,
           s.ses_date,
           m.map_name,
           a.app_time,
           d.doc_name,
           d.doc_surname,
           d.doc_patronymic,
           p.pat_name,
           p.pat_surname,
           p.pat_patronymic
      FROM session_result sr
           INNER JOIN
           session s ON sr.sres_id = s.ses_id
           INNER JOIN
           map m ON s.ses_map = m.map_id
           INNER JOIN
           appointment a ON s.ses_appointment = a.app_id
           INNER JOIN
           doctor d ON a.app_doctor = d.doc_id
           INNER JOIN
           patient p ON a.app_patient = p.pat_id;


COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
