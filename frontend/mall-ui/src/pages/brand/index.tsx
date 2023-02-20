import XBacktotop from '@/components/backToTop';
import LinearProgress from '@/components/loading/linear';
import u from '@/utils';
import { BrandDto } from '@/utils/models';
import { Alert, Container } from '@mui/material';
import { List, Image } from 'antd-mobile';
import pinyin from 'pinyin';
import * as React from 'react';
import { history } from 'umi';

export default function PinnedSubheaderList(props: any) {
  const [data, _data] = React.useState<BrandDto[]>([]);
  const [loading, _loading] = React.useState(false);

  const queryList = () => {
    _loading(true);
    u.http.apiRequest
      .post('/mall/brand/all')
      .then((res) => {
        _data(res.data.Data || []);
      })
      .finally(() => {
        _loading(false);
      });
  };

  React.useEffect(() => {
    queryList();
  }, []);

  const convertPinyin = (name: any) => {
    var pyArr = pinyin(name || '', {
      style: pinyin.STYLE_FIRST_LETTER,
    });
    var py = u.flatMap(pyArr, (x) => x) || [];
    var str = py.join('');
    return u.toString(str);
  };

  var convertedData = u.map(data, (x) => ({
    ...x,
    pinyin: u.toString(convertPinyin(x.Name)).toUpperCase(),
    py: '',
  }));
  convertedData = u.map(convertedData, (x) => ({
    ...x,
    py: (x.pinyin || '').substring(0, 1),
  }));

  const groupedData = u.groupBy(convertedData, (x) => x.py);
  const sortedKeys = u.sortBy(Object.keys(groupedData), (x) => x);

  //console.log(convertedData, groupedData, sortedKeys);
  const renderPicture = (x: BrandDto) => {
    const picUrl = u.resolveUrlv2(x.Picture, {
      width: 50,
      height: 50,
    });
    if (!picUrl) {
      return null;
    }
    return (
      <div style={{ padding: 5 }}>
        <Image
          style={{ borderRadius: 4 }}
          src={picUrl}
          width={40}
          height={40}
          lazy
          fit="contain"
          alt={x.Name || ''}
        />
      </div>
    );
  };

  return (
    <>
      <XBacktotop {...props} />
      <Container disableGutters maxWidth="sm">
        {loading && <LinearProgress />}
        {!loading && u.isEmpty(sortedKeys) && <Alert>暂无数据</Alert>}
        {u.isEmpty(sortedKeys) || (
          <div style={{}}>
            {sortedKeys.map((group, index) => {
              var groupedList = groupedData[group];
              if (u.isEmpty(groupedList)) {
                return null;
              }
              return (
                <div key={`group-${index}`} style={{}}>
                  <div
                    style={{
                      backgroundColor: '#f5f5f5',
                      color: '#999999',
                      padding: 12,
                      fontSize: '12px',
                    }}
                  >{`${group}`}</div>
                  <List>
                    {groupedList.map((x, i) => (
                      <List.Item
                        key={`item-${i}`}
                        extra={renderPicture(x)}
                        //arrow={false}
                        onClick={() => {
                          x.Id &&
                            history.push({
                              pathname: '/goods',
                              query: {
                                brand: x.Id.toString(),
                              },
                            });
                        }}
                      >
                        {x.Name}
                      </List.Item>
                    ))}
                  </List>
                </div>
              );
            })}
          </div>
        )}
      </Container>
    </>
  );
}
