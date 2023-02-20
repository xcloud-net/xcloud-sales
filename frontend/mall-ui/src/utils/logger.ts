import http from './http';

interface LogType {
  ActivityLogTypeId?: number | undefined;
  Comment?: string | undefined;
  Data?: string | undefined;
  SubjectType?: string | undefined;
  SubjectId?: string | undefined;
  SubjectIntId?: number | undefined;
}

export default {
  log(entity: LogType) {
    return new Promise<any>((resolve, reject) => {
      if (!entity) {
        reject('No entity provided');
        return;
      }
      http.apiRequest
        .post('/mall/common/save-activity-log', {
          ...entity,
        })
        .then((res) => {
          console.log(res.data);
          resolve(res);
        })
        .catch((e) => {
          console.log(e);
          reject(e);
        });
    });
  },
};
