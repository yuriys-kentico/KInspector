import axios from 'axios';
import { actionService } from './action-service';
import { reportService } from './report-service';

export default {
  actionService,
  reportService,
  getInstances() {
    return new Promise(resolve => {
      axios
        .get('/api/instances')
        .then(r => r.data)
        .then(instances => {
          resolve(instances);
        });
    });
  },

  upsertInstance(instance) {
    return new Promise((resolve, reject) => {
      axios
        .post('/api/instances', instance)
        .then(r => r.data)
        .catch(reject)
        .then(instance => {
          resolve(instance);
        });
    });
  },

  deleteInstance(guid) {
    return new Promise(resolve => {
      axios
        .delete(`/api/instances/${guid}`)
        .then(r => r.data)
        .then(result => {
          resolve(result);
        });
    });
  },

  getInstanceDetails(guid) {
    return new Promise((resolve, reject) => {
      axios
        .get(`/api/instances/details/${guid}`)
        .then(r => r.data)
        .catch(reason => {
          reject({ message: 'Error Connecting', response: reason.response });
        })
        .then(result => {
          resolve(result);
        });
    });
  }
};