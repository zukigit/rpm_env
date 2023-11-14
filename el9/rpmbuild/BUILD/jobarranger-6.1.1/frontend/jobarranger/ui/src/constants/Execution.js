export const RUN_TYPE = {
  IMMEDIATE_RUN: 1,
  IMMEDIATE_RUN_HOLD: 2,
  TEST_RUN: 3,
  SINGLE_JOB_RUN: 5,
};

export const LOAD_STATUS_TYPE = {
  NONE: 0,
  LOAD_ERR: 1,
  DELAY: 2,
  SKIP: 3,
};

export const RUN_JOB_METHOD_TYPE = {
  NORMAL: 0,
  HOLD: 1,
  SKIP: 2,
  STOP: 3,
  RERUN: 4,
};

export const RUN_JOB_TIMEOUT_TYPE = {
  NORMAL: 0,
  TIMEOUT: 1,
};

export const RUN_JOB_STATUS_TYPE = {
  NONE: 0,
  PREPARE: 1,
  DURING: 2,
  NORMAL: 3,
  RUN_ERR: 4,
  ABNORMAL: 5,
  FORCE_STOP: 6,
};

export const START_PEND_FLAG = {
  NORMAL: 0,
  PENDING: 1,
  RELEASE: 2,
};

export const EXECUTION_MANAGEMENT = "EXECUTION_MANAGEMENT";
