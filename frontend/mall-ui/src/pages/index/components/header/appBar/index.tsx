import u from '@/utils';
import { AppBar, Box, Container, Stack, Toolbar, alpha } from '@mui/material';
import XUserAvatar from './avatar';
import XTopLogo from './topLogo';

const index = function (props: any) {
  return (
    <>
      <AppBar
        position="static"
        sx={(theme) => ({
          boxShadow: 'none',
          backdropFilter: 'blur(6px)',
          WebkitBackdropFilter: 'blur(6px)', // Fix on Mobile
          backgroundColor: alpha('rgb(254,254,254)', 0.72),
        })}
      >
        <Container disableGutters maxWidth="sm">
          <Toolbar
            sx={(theme) => ({
              color: 'black',
              minHeight: u.config.app.layout.APPBAR_MOBILE,
              [theme.breakpoints.up('lg')]: {
                //minHeight: u.config.app.layout.APPBAR_DESKTOP,
                //padding: theme.spacing(0, 5),
              },
            })}
          >
            <XTopLogo />
            <Box sx={{ flexGrow: 1 }} />
            <Stack
              direction="row"
              alignItems="center"
              spacing={{ xs: 0.5, sm: 1.5 }}
            >
              <XUserAvatar />
            </Stack>
          </Toolbar>
        </Container>
      </AppBar>
    </>
  );
};

export default index;
