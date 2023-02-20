import u from '@/utils';
import { Box } from '@mui/material';
import XCallback from './auth';

export default (props: any) => {
  const { children } = props;

  const renderMask = () => {
    return (
      <Box
        sx={{
          position: 'relative',
          m: 0,
          p: 0,
        }}
      >
        {children}
        <Box
          sx={{
            position: 'absolute',
            left: 0,
            top: 0,
            width: '100%',
            height: '100%',
            zIndex: 99999,
          }}
          onClick={(e) => {
            e.preventDefault();
            if (confirm('未登录，现在去登录？')) {
              u.redirectToLogin();
            }
          }}
        ></Box>
      </Box>
    );
  };

  return <XCallback unauth={(x: any) => renderMask()}>{children}</XCallback>;
};
