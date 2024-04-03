PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: address
DROP TABLE IF EXISTS address;

CREATE TABLE IF NOT EXISTS address (
    addr_id        INTEGER PRIMARY KEY AUTOINCREMENT,
    addr_region    TEXT    NOT NULL,
    addr_street    TEXT    NOT NULL,
    addr_house     INTEGER NOT NULL,
    addr_apartment INTEGER,
    addr_corpus    INTEGER DEFAULT (1) 
                           NOT NULL,
    addr_district  TEXT    NOT NULL
);


-- Table: appointment
DROP TABLE IF EXISTS appointment;

CREATE TABLE IF NOT EXISTS appointment (
    app_id      INTEGER PRIMARY KEY AUTOINCREMENT,
    app_time    TEXT    NOT NULL,
    app_doctor  INTEGER REFERENCES doctor (doc_id) ON DELETE RESTRICT
                                                   ON UPDATE CASCADE
                        NOT NULL,
    app_patient INTEGER REFERENCES patient (pat_id) ON DELETE RESTRICT
                                                    ON UPDATE CASCADE
                        NOT NULL,
    app_cabinet INTEGER REFERENCES doctor_cabinet (dc_id) ON DELETE RESTRICT
                                                          ON UPDATE CASCADE
                        NOT NULL
);


-- Table: card
DROP TABLE IF EXISTS card;

CREATE TABLE IF NOT EXISTS card (
    crd_id      INTEGER  PRIMARY KEY AUTOINCREMENT,
    crd_patient INTEGER  REFERENCES patient (pat_id) ON DELETE RESTRICT
                                                     ON UPDATE CASCADE
                         NOT NULL,
    crd_number  TEXT (9) UNIQUE
                         NOT NULL
);


-- Table: contraindication
DROP TABLE IF EXISTS contraindication;

CREATE TABLE IF NOT EXISTS contraindication (
    con_id   INTEGER PRIMARY KEY AUTOINCREMENT,
    con_card INTEGER REFERENCES card (crd_id) ON DELETE CASCADE
                                              ON UPDATE CASCADE
                     NOT NULL,
    con_name TEXT    NOT NULL
);


-- Table: diagnosis
DROP TABLE IF EXISTS diagnosis;

CREATE TABLE IF NOT EXISTS diagnosis (
    dia_id   INTEGER PRIMARY KEY AUTOINCREMENT
                     UNIQUE
                     NOT NULL,
    dia_name TEXT    UNIQUE
                     NOT NULL
);


-- Table: doctor
DROP TABLE IF EXISTS doctor;

CREATE TABLE IF NOT EXISTS doctor (
    doc_id         INTEGER PRIMARY KEY AUTOINCREMENT
                           NOT NULL
                           UNIQUE,
    doc_name       TEXT    NOT NULL,
    doc_surname    TEXT    NOT NULL,
    doc_patronymic TEXT,
    doc_password   TEXT    NOT NULL
);


-- Table: doctor_cabinet
DROP TABLE IF EXISTS doctor_cabinet;

CREATE TABLE IF NOT EXISTS doctor_cabinet (
    dc_id          INTEGER PRIMARY KEY AUTOINCREMENT,
    dc_floor       INTEGER NOT NULL,
    dc_cabinet_num INTEGER NOT NULL,
    dc_hospital    INTEGER REFERENCES hospital (h_id) ON DELETE CASCADE
                                                      ON UPDATE CASCADE
                           NOT NULL
);


-- Table: hospital
DROP TABLE IF EXISTS hospital;

CREATE TABLE IF NOT EXISTS hospital (
    h_id        INTEGER PRIMARY KEY AUTOINCREMENT,
    hsp_address INTEGER REFERENCES address (addr_id) ON DELETE RESTRICT
                                                     ON UPDATE CASCADE
                        NOT NULL
                        UNIQUE,
    hsp_name    TEXT    NOT NULL
);


-- Table: m2m_card_diagnosis
DROP TABLE IF EXISTS m2m_card_diagnosis;

CREATE TABLE IF NOT EXISTS m2m_card_diagnosis (
    c2d_card             INTEGER REFERENCES card (crd_id) ON DELETE CASCADE
                                                          ON UPDATE CASCADE
                                 NOT NULL,
    c2d_diagnosis        INTEGER REFERENCES diagnosis (dia_id) ON DELETE RESTRICT
                                                               ON UPDATE CASCADE
                                 NOT NULL,
    c2d_diagnosis_start  TEXT    NOT NULL,
    c2d_diagnosis_finish TEXT,
    PRIMARY KEY (
        c2d_card,
        c2d_diagnosis
    )
);


-- Table: map
DROP TABLE IF EXISTS map;

CREATE TABLE IF NOT EXISTS map (
    map_id               INTEGER PRIMARY KEY AUTOINCREMENT
                                 UNIQUE
                                 NOT NULL,
    map_coordinates_json TEXT    NOT NULL,
    map_created_at       TEXT    NOT NULL,
    map_created_by       INTEGER REFERENCES doctor (doc_id) ON DELETE CASCADE
                                                            ON UPDATE CASCADE
                                 NOT NULL
);


-- Table: note
DROP TABLE IF EXISTS note;

CREATE TABLE IF NOT EXISTS note (
    nt_id      INTEGER PRIMARY KEY AUTOINCREMENT,
    nt_patient INTEGER REFERENCES patient (pat_id) ON DELETE CASCADE
                                                   ON UPDATE CASCADE
                       NOT NULL,
    nt_doctor  INTEGER REFERENCES doctor (doc_id) ON DELETE CASCADE
                                                  ON UPDATE CASCADE
                       NOT NULL,
    nt_text    TEXT    NOT NULL
);


-- Table: operation
DROP TABLE IF EXISTS operation;

CREATE TABLE IF NOT EXISTS operation (
    op_id          INTEGER PRIMARY KEY AUTOINCREMENT,
    op_card                REFERENCES card (crd_id) ON DELETE CASCADE
                                                    ON UPDATE CASCADE
                           NOT NULL,
    op_asingned_by INTEGER REFERENCES doctor (doc_id) ON DELETE RESTRICT
                                                      ON UPDATE CASCADE,
    op_name        TEXT    NOT NULL
);


-- Table: path_in_target
DROP TABLE IF EXISTS path_in_target;

CREATE TABLE IF NOT EXISTS path_in_target (
    pit_session          INTEGER REFERENCES session (ses_id) ON DELETE RESTRICT
                                                             ON UPDATE CASCADE
                                 NOT NULL,
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
    ptt_session          INTEGER REFERENCES session (ses_id) ON DELETE RESTRICT
                                                             ON UPDATE CASCADE
                                 NOT NULL,
    ptt_num              INTEGER NOT NULL,
    ptt_coordinates_json TEXT    NOT NULL,
    PRIMARY KEY (
        ptt_session,
        ptt_num
    )
);


-- Table: patient
DROP TABLE IF EXISTS patient;

CREATE TABLE IF NOT EXISTS patient (
    pat_id            INTEGER   PRIMARY KEY AUTOINCREMENT,
    pat_name          TEXT (20) NOT NULL,
    pat_surname       TEXT (30) NOT NULL,
    pat_patronymic    TEXT (30),
    pat_address       INTEGER   REFERENCES address (addr_id) ON DELETE RESTRICT
                                                             ON UPDATE CASCADE
                                NOT NULL,
    pat_date_of_birth TEXT      NOT NULL,
    pat_phone_mobile  TEXT      NOT NULL,
    pat_phone_home    TEXT
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
    pro_date_time   TEXT    NOT NULL,
    pro_name        TEXT    NOT NULL
);


-- Table: session
DROP TABLE IF EXISTS session;

CREATE TABLE IF NOT EXISTS session (
    ses_id            INTEGER PRIMARY KEY AUTOINCREMENT
                              UNIQUE
                              NOT NULL,
    ses_map           INTEGER REFERENCES map (map_id) ON DELETE RESTRICT
                                                      ON UPDATE CASCADE
                              NOT NULL,
    ses_log_file_path TEXT    NOT NULL
                              UNIQUE,
    ses_date          TEXT    NOT NULL,
    ses_appointment   INTEGER REFERENCES appointment (app_id) ON DELETE RESTRICT
                                                              ON UPDATE CASCADE
                              NOT NULL
);


-- Table: session_result
DROP TABLE IF EXISTS session_result;

CREATE TABLE IF NOT EXISTS session_result (
    sres_id         INTEGER PRIMARY KEY
                            REFERENCES session (ses_id) ON DELETE RESTRICT
                                                        ON UPDATE CASCADE
                            NOT NULL
                            UNIQUE,
    sres_math_exp   REAL    NOT NULL,
    sres_deviation  REAL    NOT NULL,
    sres_dispersion REAL    NOT NULL
);


-- Table: target_file
DROP TABLE IF EXISTS target_file;

CREATE TABLE IF NOT EXISTS target_file (
    tf_id       INTEGER PRIMARY KEY AUTOINCREMENT,
    tf_filepath TEXT    NOT NULL
                        UNIQUE,
    tf_added_by         REFERENCES doctor (doc_id) ON DELETE CASCADE
                                                   ON UPDATE CASCADE
                        NOT NULL
);


-- Table: xray
DROP TABLE IF EXISTS xray;

CREATE TABLE IF NOT EXISTS xray (
    xr_id          INTEGER PRIMARY KEY AUTOINCREMENT,
    xr_date        TEXT    NOT NULL,
    xr_file_path   TEXT    NOT NULL
                           UNIQUE,
    xr_description TEXT,
    xr_card        INTEGER REFERENCES card (crd_id) ON DELETE CASCADE
                                                    ON UPDATE CASCADE
                           NOT NULL
);


COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
