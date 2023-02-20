import { Box } from '@mui/material';

export default ({
  children,
  visiable,
}: {
  children: any;
  visiable: boolean;
}) => {
  if (!visiable) {
    return children;
  }
  return (
    <Box
      component={'em'}
      sx={{
        display: 'inline-block',
        backgroundColor: '#ffdb00',
        boxShadow: '0.08em 0.08em #cc0008',
        padding: '2px 4px',
      }}
    >
      {children}
    </Box>
  );
};
