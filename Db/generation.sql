--
-- File generated with SQLiteStudio v3.4.4 on ѕн май 20 16:28:38 2024
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: appointment
DROP TABLE IF EXISTS appointment;

CREATE TABLE IF NOT EXISTS appointment (
    app_id        INTEGER PRIMARY KEY AUTOINCREMENT,
    app_date_time TEXT    NOT NULL,
    app_doctor    INTEGER REFERENCES doctor (doc_id) ON DELETE CASCADE
                                                     ON UPDATE CASCADE
                          NOT NULL,
    app_patient   NUMERIC REFERENCES patient (pat_id) ON DELETE CASCADE
                                                      ON UPDATE CASCADE
                          NOT NULL
);


-- Table: doctor
DROP TABLE IF EXISTS doctor;

CREATE TABLE IF NOT EXISTS doctor (
    doc_id         INTEGER PRIMARY KEY AUTOINCREMENT,
    doc_name       TEXT    NOT NULL
                           COLLATE NOCASE,
    doc_surname    TEXT    NOT NULL
                           COLLATE NOCASE,
    doc_patronymic TEXT    COLLATE NOCASE,
    doc_password   TEXT    NOT NULL
);


-- Table: map
DROP TABLE IF EXISTS map;

CREATE TABLE IF NOT EXISTS map (
    map_id                   INTEGER PRIMARY KEY AUTOINCREMENT,
    map_coordinates_json     TEXT    NOT NULL,
    map_created_at_date_time TEXT    NOT NULL,
    map_created_by           INTEGER REFERENCES doctor (doc_id) ON DELETE CASCADE
                                                                ON UPDATE CASCADE
                                     NOT NULL,
    map_name                 TEXT    UNIQUE
                                     NOT NULL
                                     COLLATE NOCASE
);


-- Table: note
DROP TABLE IF EXISTS note;

CREATE TABLE IF NOT EXISTS note (
    nt_id      INTEGER PRIMARY KEY AUTOINCREMENT,
    nt_doctor  INTEGER REFERENCES doctor (doc_id) 
                       NOT NULL,
    nt_patient INTEGER REFERENCES patient (pat_id) 
                       NOT NULL,
    nt_text    TEXT    NOT NULL
);


-- Table: path_in_target
DROP TABLE IF EXISTS path_in_target;

CREATE TABLE IF NOT EXISTS path_in_target (
    pit_session          INTEGER REFERENCES session (ses_id) ON DELETE RESTRICT
                                                             ON UPDATE CASCADE,
    pit_target_id        INTEGER NOT NULL,
    pit_coordinates_json TEXT    NOT NULL,
    PRIMARY KEY (
        pit_session,
        pit_target_id
    )
);


-- Table: path_to_target
DROP TABLE IF EXISTS path_to_target;

CREATE TABLE IF NOT EXISTS path_to_target (
    ptt_session                  REFERENCES session (ses_id) ON DELETE RESTRICT
                                                             ON UPDATE CASCADE
                                 NOT NULL,
    ptt_target_num       INTEGER NOT NULL,
    ptt_coordinates_json TEXT    NOT NULL,
    ptt_ange_distance    REAL    NOT NULL,
    ptt_angle_speed      REAL    NOT NULL,
    ptt_approach_speed   REAL    NOT NULL,
    ptt_time             REAL    NOT NULL,
    PRIMARY KEY (
        ptt_session,
        ptt_target_num
    )
);


-- Table: patient
DROP TABLE IF EXISTS patient;

CREATE TABLE IF NOT EXISTS patient (
    pat_id            INTEGER   PRIMARY KEY AUTOINCREMENT,
    pat_name          TEXT (20) NOT NULL
                                COLLATE NOCASE,
    pat_surname       TEXT (30) NOT NULL
                                COLLATE NOCASE,
    pat_patronymic    TEXT (30) COLLATE NOCASE,
    pat_date_of_birth TEXT      NOT NULL,
    pat_phone_mobile  TEXT      NOT NULL,
    pat_phone_home    TEXT
);


-- Table: session
DROP TABLE IF EXISTS session;

CREATE TABLE IF NOT EXISTS session (
    ses_id            INTEGER PRIMARY KEY AUTOINCREMENT,
    ses_map           INTEGER REFERENCES map (map_id) ON DELETE RESTRICT
                                                      ON UPDATE CASCADE
                              NOT NULL,
    ses_log_file_path TEXT    NOT NULL
                              UNIQUE,
    ses_date_time     TEXT    NOT NULL,
    ses_appointment   INTEGER REFERENCES appointment (app_id) 
                              NOT NULL
);


-- Table: session_result
DROP TABLE IF EXISTS session_result;

CREATE TABLE IF NOT EXISTS session_result (
    sres_id                 PRIMARY KEY
                            REFERENCES session (ses_id) ON DELETE RESTRICT
                                                        ON UPDATE CASCADE,
    sres_math_exp   REAL    NOT NULL,
    sres_deviation  REAL    NOT NULL,
    sres_dispersion REAL    NOT NULL,
    sres_score      INTEGER NOT NULL
);


COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
