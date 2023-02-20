import u from '@/utils';
import http from '@/utils/http';
import { Grid } from '@mui/material';
import { message, Modal } from 'antd';
import { useEffect, useState } from 'react';
import XJson from './json';
import XPreview from './preview';

export default function (props: any) {
  const { show, hide, data, ok } = props;
  const [loading, _loading] = useState(false);

  const [originJsonData, _originJsonData] = useState({});
  const [jsonData, _jsonData] = useState({});

  const save = () => {
    _loading(true);

    http.apiRequest
      .post('/mall-admin/pages/set-content', {
        Id: data.Id,
        Content: JSON.stringify(jsonData),
      })
      .then((res) => {
        u.handleResponse(res, () => {
          message.success('保存成功');
          ok && ok();
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  const parseJsonObject = () => {
    try {
      var jsonModel = JSON.parse(data.BodyContent || '{}');
      return jsonModel;
    } catch (e) {
      console.log(e, 'set body content');
    }

    return {};
  };

  useEffect(() => {
    data && _originJsonData(parseJsonObject());
    data && _jsonData(parseJsonObject());
  }, [data]);

  return (
    <>
      <Modal
        title="编辑页面"
        open={show}
        confirmLoading={loading}
        onCancel={() => hide()}
        onOk={() => save()}
        width="100%"
      >
        <Grid container spacing={2} sx={{ backgroundColor: 'white' }}>
          <Grid item xs={7}>
            <div style={{ height: 600 }}>
              <XJson
                json={originJsonData}
                ok={(e: string) => {
                  try {
                    console.log(e);
                    _jsonData(JSON.parse(e));
                  } catch (err) {
                    console.log(err);
                  }
                }}
              />
            </div>
          </Grid>
          <Grid item xs={5}>
            <XPreview data={jsonData} />
          </Grid>
        </Grid>
      </Modal>
    </>
  );
}
