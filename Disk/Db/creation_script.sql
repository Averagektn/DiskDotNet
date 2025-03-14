--
-- File generated with SQLiteStudio v3.4.15 on пт март 14 14:26:36 2025
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: attempt
DROP TABLE IF EXISTS attempt;

CREATE TABLE IF NOT EXISTS attempt (
    att_id            INTEGER NOT NULL
                              CONSTRAINT PK_attempt PRIMARY KEY AUTOINCREMENT,
    att_max_x_angle   REAL    NOT NULL,
    att_max_y_angle   REAL    NOT NULL,
    att_cursor_radius INTEGER NOT NULL,
    att_target_radius INTEGER NOT NULL,
    att_log_file_path TEXT    NOT NULL,
    att_date_time     TEXT    NOT NULL,
    att_session       INTEGER NOT NULL,
    CONSTRAINT FK_attempt_session_att_session FOREIGN KEY (
        att_session
    )
    REFERENCES session (ses_id) ON DELETE CASCADE
);


-- Table: attempt_result
DROP TABLE IF EXISTS attempt_result;

CREATE TABLE IF NOT EXISTS attempt_result (
    ares_id          INTEGER NOT NULL
                             CONSTRAINT PK_attempt_result PRIMARY KEY,
    ares_math_exp_y  REAL    NOT NULL,
    ares_math_exp_x  REAL    NOT NULL,
    ares_deviation_x REAL    NOT NULL,
    ares_deviation_y REAL    NOT NULL,
    ares_score       INTEGER NOT NULL,
    CONSTRAINT FK_attempt_result_attempt_ares_id FOREIGN KEY (
        ares_id
    )
    REFERENCES attempt (att_id) ON DELETE CASCADE
);


-- Table: map
DROP TABLE IF EXISTS map;

CREATE TABLE IF NOT EXISTS map (
    map_id                   INTEGER NOT NULL
                                     CONSTRAINT PK_map PRIMARY KEY AUTOINCREMENT,
    map_coordinates_json     TEXT    NOT NULL,
    map_created_at_date_time TEXT    NOT NULL,
    map_name                 TEXT    COLLATE NOCASE
                                     NOT NULL,
    map_description          TEXT    NULL
);


-- Table: path_in_target
DROP TABLE IF EXISTS path_in_target;

CREATE TABLE IF NOT EXISTS path_in_target (
    pit_session          INTEGER NOT NULL,
    pit_target_id        INTEGER NOT NULL,
    pit_coordinates_json TEXT    NOT NULL,
    pit_precision        REAL    NOT NULL,
    CONSTRAINT PK_path_in_target PRIMARY KEY (
        pit_session,
        pit_target_id
    ),
    CONSTRAINT FK_path_in_target_attempt_pit_session FOREIGN KEY (
        pit_session
    )
    REFERENCES attempt (att_id) ON DELETE CASCADE
);


-- Table: path_to_target
DROP TABLE IF EXISTS path_to_target;

CREATE TABLE IF NOT EXISTS path_to_target (
    ptt_session          INTEGER NOT NULL,
    ptt_target_id        INTEGER NOT NULL,
    ptt_coordinates_json TEXT    NOT NULL,
    ptt_distance         REAL    NOT NULL,
    ptt_average_speed    REAL    NOT NULL,
    ptt_approach_speed   REAL    NOT NULL,
    ptt_time             REAL    NOT NULL,
    CONSTRAINT PK_path_to_target PRIMARY KEY (
        ptt_session,
        ptt_target_id
    ),
    CONSTRAINT FK_path_to_target_attempt_ptt_session FOREIGN KEY (
        ptt_session
    )
    REFERENCES attempt (att_id) ON DELETE CASCADE
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
    pat_patronymic    TEXT (30) COLLATE NOCASE
                                NULL,
    pat_date_of_birth TEXT      NOT NULL,
    pat_phone_mobile  TEXT      NOT NULL,
    pat_phone_home    TEXT      NULL
);


-- Table: session
DROP TABLE IF EXISTS session;

CREATE TABLE IF NOT EXISTS session (
    ses_id      INTEGER NOT NULL
                        CONSTRAINT PK_session PRIMARY KEY AUTOINCREMENT,
    ses_map     INTEGER NOT NULL,
    ses_patient INTEGER NOT NULL,
    ses_date    TEXT    NOT NULL,
    CONSTRAINT FK_session_map_ses_map FOREIGN KEY (
        ses_map
    )
    REFERENCES map (map_id) ON DELETE RESTRICT,
    CONSTRAINT FK_session_patient_ses_patient FOREIGN KEY (
        ses_patient
    )
    REFERENCES patient (pat_id) ON DELETE CASCADE
);


COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
