import XSearchIcon from './searchIcon';
import u from '@/utils';
import { ChevronLeft } from '@mui/icons-material';
import { Container, IconButton, Stack, alpha } from '@mui/material';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Slide from '@mui/material/Slide';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import useScrollTrigger from '@mui/material/useScrollTrigger';
import { useSize } from 'ahooks';
import React, { useRef } from 'react';
import { history, useModel } from 'umi';

export default function ButtonAppBar(props: {
  children?: any;
  route?: any;
  window?: any;
}) {
  const { children, route, window } = props;

  const trigger = useScrollTrigger({
    target: window ? window() : undefined,
    threshold: 100,
  });

  const appSettingModel = useModel('storeAppSetting');
  const ref = useRef(null);
  const rect = useSize(ref);

  React.useEffect(() => {
    if (trigger) {
      appSettingModel._headerHeight(0);
    } else {
      appSettingModel._headerHeight(rect?.height || 0);
    }
  }, [rect, trigger]);

  return (
    <Box sx={{ flexGrow: 1 }}>
      <Slide appear={false} direction="down" in={!trigger}>
        <AppBar
          ref={ref}
          sx={(theme) => ({
            boxShadow: 'none',
            backdropFilter: 'blur(6px)',
            WebkitBackdropFilter: 'blur(6px)', // Fix on Mobile
            backgroundColor: alpha('#ffffff', 0.72),
            zIndex: 1,
          })}
          position={'fixed'}
        >
          <Container maxWidth="sm" disableGutters>
            <Toolbar
              sx={(theme) => ({
                color: 'black',
                minHeight: u.config.app.layout.APPBAR_MOBILE,
              })}
            >
              <Stack
                direction="row"
                alignItems="center"
                spacing={{ xs: 0.5, sm: 1.5 }}
              >
                <IconButton
                  size="large"
                  color="inherit"
                  onClick={() => {
                    if (history.length > 1) {
                      history.goBack();
                    } else {
                      history.push({
                        pathname: '/',
                      });
                    }
                  }}
                >
                  <ChevronLeft />
                </IconButton>
              </Stack>
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'row',
                  alignItems: 'center',
                  justifyContent: 'center',
                }}
                flexGrow={1}
              >
                <Typography variant="h6">
                  {route.title || u.config.app.name}
                </Typography>
              </Box>
              <Stack
                direction="row"
                alignItems="center"
                spacing={{ xs: 0.5, sm: 1.5 }}
              >
                <XSearchIcon />
              </Stack>
            </Toolbar>
          </Container>
        </AppBar>
      </Slide>

      <Box sx={{ mt: `${u.config.app.layout.APPBAR_MOBILE + 10}px` }}>
        {children}
      </Box>
    </Box>
  );
}
