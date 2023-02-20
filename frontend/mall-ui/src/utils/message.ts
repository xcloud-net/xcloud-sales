import toast from 'react-hot-toast';

export default {
  error(msg: string) {
    toast.error(msg);
  },
  success(msg: string) {
    toast.success(msg);
  },
};
