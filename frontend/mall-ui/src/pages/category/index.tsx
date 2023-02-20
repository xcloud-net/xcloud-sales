import LinearProgress from '@/components/loading/linear';
import u from '@/utils';
import http from '@/utils/http';
import { CategoryDto } from '@/utils/models';
import { Alert, Badge, Container, styled } from '@mui/material';
import Box from '@mui/material/Box';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import * as React from 'react';
import { history, useModel } from 'umi';
import XCategoryDetail from './categoryDetail';
import XSimpleHeader from './components/header';
import XGoods from './goods';

const XTab = styled(Tab)((theme) => ({
  '&.Mui-selected': {
    color: '#282b31',
  },
}));

const XTabs = styled(Tabs)((theme) => ({
  '.MuiTabs-indicator': {
    backgroundColor: u.color.primary,
    width: '1px',
  },
}));

export default function VerticalTabs(props: any) {
  const [loading, _loading] = React.useState(false);
  const [data, _data] = React.useState<CategoryDto[]>([]);
  const [selectedId, _selectedId] = React.useState(0);
  const [filterdId, _filteredId] = React.useState(0);

  const selectedModel = u.find(data, (x) => x.Id == selectedId);

  const appSettingModel = useModel('storeAppSetting');

  const queryCategoryPageData = () => {
    _loading(true);
    http.apiRequest
      .post('/mall/category/view')
      .then((res) => {
        u.handleResponse(res, () => {
          _data(res.data.Data?.Root || []);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  const trySelectTab = (res: any, id: number) => {
    if (u.isEmpty(res)) {
      return;
    }
    var selectedParent = u.first(u.sortBy(res, (x) => (x.Id == id ? 0 : 1)));
    selectedParent && _selectedId(selectedParent.Id);
    //u.isEmpty(res) || _selectedId(res[0].Id);
  };

  React.useEffect(() => {
    var id = parseInt((history.location.query?.cat as string) || '0');
    trySelectTab(data, id);
  }, [data]);

  React.useEffect(() => {
    queryCategoryPageData();
  }, []);

  React.useEffect(() => {
    _filteredId(selectedId);
    window && window.scrollTo({ top: 0, left: 0 });
  }, [selectedId]);

  React.useEffect(() => {
    var originColor = document.body.style.backgroundColor;
    document.body.style.backgroundColor = 'white';
    return () => {
      document.body.style.backgroundColor = originColor;
    };
  }, []);

  const renderCategories = () => {
    if (u.isEmpty(data)) {
      return null;
    }
    return (
      <XTabs
        orientation="vertical"
        variant="scrollable"
        scrollButtons={false}
        value={selectedId}
        onChange={(event: React.SyntheticEvent, newValue: number) => {
          _selectedId(newValue);
        }}
        sx={{
          backgroundColor: 'rgb(251,251,251)',
          height: '100%',
        }}
      >
        {u.map(data, (x, index) => {
          return (
            <XTab
              key={index}
              value={x.Id}
              label={
                <Badge
                  badgeContent="荐"
                  color="error"
                  variant="dot"
                  invisible={!x.Recommend}
                >
                  <span>{x.Name}</span>
                </Badge>
              }
            />
          );
        })}
      </XTabs>
    );
  };

  return (
    <>
      <XSimpleHeader {...props}>
        <Container disableGutters maxWidth="sm">
          <Box sx={{}}>
            {loading && <LinearProgress />}
            {loading || <>{u.isEmpty(data) && <Alert>暂无类目</Alert>}</>}

            <Box
              sx={{
                display: 'flex',
                flexDirection: 'row',
                justifyContent: 'flex-start',
              }}
            >
              <Box
                sx={{
                  width: '150px',
                }}
              >
                <Box
                  sx={(theme) => ({
                    position: 'fixed',
                    top: appSettingModel.headerHeight,
                    bottom: appSettingModel.bottomHeight,
                    overflow: 'hidden',
                    zIndex: 1,
                  })}
                >
                  {renderCategories()}
                </Box>
              </Box>
              <Box
                sx={{
                  width: '100%',
                  minHeight: '100%',
                }}
              >
                {selectedModel == null || (
                  <XCategoryDetail
                    model={selectedModel}
                    selectedId={filterdId}
                    onSelect={(e: number | undefined) => {
                      const childId = e || 0;
                      if (childId > 0) {
                        _filteredId(childId);
                      } else {
                        _filteredId(selectedId);
                      }
                    }}
                  />
                )}
                {filterdId > 0 && <XGoods categoryId={filterdId} />}
              </Box>
            </Box>
          </Box>
        </Container>
      </XSimpleHeader>
    </>
  );
}
