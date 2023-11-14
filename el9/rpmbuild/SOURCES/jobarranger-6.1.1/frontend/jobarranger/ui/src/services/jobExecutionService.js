import http from '../common/http'

const JobExecutionService = {
    checkValid: (data = {}) => {
        return http.post(`/exec/checkValid`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    run: (data = {}) => {
        return http.post(`/exec/run`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    singleJobRun: (data = {}) => {
        return http.post(`/exec/singleRun`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    getRunJobnetData: (data = {}) => {
        return http.post(`/exec/getData`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    setSkipJob: (data = {}) => {
        return http.post(`/exec/skip`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    setHoldJob: (data = {}) => {
        return http.post(`/exec/hold`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    setNormalJob: (data = {}) => {
        return http.post(`/exec/normal`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    setRerun: (data = {}) => {
        return http.post(`/exec/rerun`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    setForceStop: (data = {}) => {
        return http.post(`/exec/forceStop`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    variableValueChange: (data = {}) => {
        return http.post(`/exec/valueChange`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    stopAllJobnetSummary: (data = {}) => {
        return http.post(`/jobExecManagement/stopAllJobnetSummary`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    stopErrorJobnetSummary: (data = {}) => {
        return http.post(`/jobExecManagement/stopErrorJobnetSummary`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    stopDuringJobnetSummary: (data = {}) => {
        return http.post(`/jobExecManagement/stopDuringJobnetSummary`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    delayJobnetSummary: (data = {}) => {
        return http.post(`/jobExecManagement/delayJobnetSummary`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    updateSchedule: (data = {}) => {
        return http.post(`/jobExecManagement/updateSchedule`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    holdJobnetSummary: (data = {}) => {
        return http.post(`/jobExecManagement/holdJobnetSummary`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    releaseJobnetSummary: (data = {}) => {
        return http.post(`/jobExecManagement/releaseJobnetSummary`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    checkScheduleValid: (data = {}) => {
        return http.post(`/jobExecManagement/checkScheduleValid`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
    deleteSchedule: (data = {}) => {
        return http.post(`/jobExecManagement/deleteSchedule`, data)
            .then(({ data }) => {
                return data
            })
            .catch(err => {
                console.log("job execution service err", err);
                throw err
            })
    },
};

export default JobExecutionService;