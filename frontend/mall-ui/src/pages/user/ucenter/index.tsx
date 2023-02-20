import { Box, Container } from '@mui/material';
import XHeader from './header';
import XMenu from './menu';
import XBalance from './balance';
import { useEffect } from 'react';
//import './index.less';

const index = function BoxSx(props: any) {
  useEffect(() => {
    var originColor = document.body.style.backgroundColor;
    document.body.style.backgroundColor = '#f5f7f9';
    return () => {
      document.body.style.backgroundColor = originColor;
    };
  }, []);

  return (
    <>
      <Container maxWidth="sm" disableGutters>
        <Box sx={{ mb: 1 }} className="bg">
          <XHeader />
          <XBalance />
          <XMenu />
        </Box>
      </Container>
    </>
  );
};

export default index;
