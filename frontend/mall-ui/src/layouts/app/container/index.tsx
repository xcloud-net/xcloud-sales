import { Container } from '@mui/material';

export default function ButtonAppBar(props: any) {
  const { children } = props;

  return (
    <Container
      disableGutters
      sx={{
        maxWidth: {
          xs: 'xs',
          sm: 'sm',
          md: 'md',
        },
        overflowX: 'hidden',
      }}
    >
      {children}
    </Container>
  );
}
