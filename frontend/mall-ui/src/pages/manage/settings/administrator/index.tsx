import XTime from '@/components/manage/time';
import XUserAvatar from '@/components/manage/user/avatar';
import u from '@/utils';
import http from '@/utils/http';
import { RoleDto, SysAdminDto } from '@/utils/models';
import { Button, Card, Table } from 'antd';
import { ColumnType } from 'antd/es/table';
import { useEffect, useState } from 'react';
import XForm from './form';
import XSearchForm from './searchForm';
import XStatus from './status';
import XRoles from './roles';

const AdminPage = (props: any) => {
  const [query, _query] = useState({
    current: 1,
  });
  const [data, _data] = useState([]);
  const [total, _total] = useState(1);
  const [loading, _loading] = useState(false);
  const [show, _show] = useState(false);

  const queryPaging = () => {
    _loading(true);
    http.adminRequest
      .post('/admin/list', {
        Page: 1,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          _data(res.data.Items || []);
          _total(res.data.TotalCount);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    queryPaging();
  }, [query]);

  const columns: ColumnType<any>[] = [
    {
      title: '账号主体',
      render: (record: SysAdminDto) => <XUserAvatar model={record.SysUser} />,
    },
    {
      title: '名称',
      fixed: 'left',
      render: (text, record) => {
        var name = record.IdentityName;
        return <a>{name}</a>;
      },
    },
    {
      title: '状态',
      render: (text, record) => (
        <XStatus
          model={record}
          ok={() => {
            queryPaging();
          }}
        />
      ),
    },
    {
      title: '时间',
      render: (x) => <XTime model={x} />,
    },
    {
      title: '操作',
      render: (text, record) => {
        return (
          <Button.Group size='small'>
            <Button type='primary' onClick={() => {
            }}>
              编辑
            </Button>
          </Button.Group>
        );
      },
    },
  ];
  return (
    <>
      <XForm
        show={show}
        hide={() => {
          _show(false);
        }}
        ok={() => {
          _show(false);
          queryPaging();
        }}
      />
      <XSearchForm
        query={query}
        onSearch={(q: any) => {
          _query(q);
        }}
      />
      <Card
        size='small'
        extra={
          <Button
            size='small'
            type='primary'
            onClick={() => {
              _show(true);
            }}
          >
            添加
          </Button>
        }
      >
        <Table
          rowKey={(x) => x.Id}
          loading={loading}
          columns={columns}
          dataSource={data}
          pagination={false}
          expandable={{
            expandedRowRender: (x) => <XRoles model={x} ok={() => {
              queryPaging();
            }} />,
          }}
        />
      </Card>
    </>
  );
};

export default AdminPage;
