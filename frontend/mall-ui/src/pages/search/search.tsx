import XSearchBox from '@/components/searchBox';
import u from '@/utils';
import { WidgetsOutlined } from '@mui/icons-material';
import { Box, Button, IconButton, Tooltip, Typography } from '@mui/material';
import { useState } from 'react';
import { history } from 'umi';

export default function CustomizedInputBase({ model }: { model: any }) {
  const [keywords, _keywords] = useState('');
  const keywordsList: any[] = model.Keywords || [];

  const triggerSearch = (kwd: any) => {
    if (u.isEmpty(kwd)) {
      u.error('请输入关键词');
      return;
    }
    history.push({
      pathname: '/goods',
      query: {
        kwd: kwd,
      },
    });
  };

  return (
    <>
      <Box
        sx={{
          px: 2,
          mb: 2,
          display: 'flex',
          flexDirection: 'row',
          alignItems: 'center',
          justifyContent: 'flex-end',
        }}
      >
        <Box
          sx={{
            width: '100%',
            mr: 1,
          }}
        >
          <XSearchBox
            keywords={keywords}
            onChange={(e: string) => {
              _keywords((e || '').substring(0, 15));
            }}
            onSearch={() => {
              triggerSearch(keywords);
            }}
          />
        </Box>
        <Tooltip title="品牌">
          <IconButton
            onClick={() => {
              history.push({
                pathname: '/brand',
              });
            }}
          >
            <WidgetsOutlined />
          </IconButton>
        </Tooltip>
      </Box>
      {u.isEmpty(keywordsList) || (
        <Box sx={{ px: 2, py: 1 }}>
          <Typography variant="overline" color="text.disabled" gutterBottom>
            热门关键词
          </Typography>
          <Box sx={{ mt: 1 }}>
            {keywordsList.map((x, index) => (
              <Button
                key={index}
                size="small"
                variant="text"
                sx={{
                  color: (theme) => theme.palette.text.primary,
                }}
                onClick={() => {
                  triggerSearch(x);
                }}
              >
                {x}
              </Button>
            ))}
          </Box>
        </Box>
      )}
    </>
  );
}
