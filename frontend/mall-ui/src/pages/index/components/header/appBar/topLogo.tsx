import logoLg from '@/assets/logo-no-background.png';
import { Box } from '@mui/material';
import { Link } from 'umi';

const index = function (props: any) {
  return (
    <>
      <Box sx={{}}>
        <Link
          to={{
            pathname: '/',
          }}
        >
          <Box sx={{}}>
            <img src={logoLg} style={{ height: '30px', width: 'auto' }} />
          </Box>
        </Link>
      </Box>
    </>
  );
};

export default index;
