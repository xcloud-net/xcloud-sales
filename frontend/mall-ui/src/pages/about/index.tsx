import { Box, Container } from '@mui/material';
import XContact from './xcontact';
import XHeader from './header';
import XService from './service';
import XFooter from '@/components/footer';
import PageTitle from '@/components/pageTitle';
import u from '@/utils';
import { Favorite } from '@mui/icons-material';

export default function Types() {
  return (
    <>
      <Container maxWidth="sm" disableGutters>
        <Box sx={{ mx: 0 }}>
          <XHeader />
          <Box sx={{ px: 1, my: 2 }}>
            <PageTitle
              title={u.config.app.name}
              desc="欢迎来到豆芽家，我们为您提供专业的美妆服务~"
              right={<Favorite color="error" />}
            />
          </Box>
          <XService />
          <XContact />
          <XFooter />
        </Box>
      </Container>
    </>
  );
}
