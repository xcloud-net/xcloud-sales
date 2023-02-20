import u from '@/utils';
import http from '@/utils/http';
import { message, Tree, Card } from 'antd';
import React, { useEffect, useState } from 'react';

const App = () => {
  const [treeData, setTreeData] = useState<Array<any>>([]);

  const [loading, _loading] = React.useState(false);

  const buildNode = (node: any) => {
    return {
      key: node.Id,
      title: node.Name,
    };
  };

  const queryRegionRequest = (parentId: string | null) => {
    if (u.toString(parentId) == '-1') {
      parentId = null;
    }

    return http.platformRequest.post('/region/by-parent', {
      Id: parentId,
    });
  };

  const queryRegion = (parentId: string | null, cb: Function) => {
    _loading(true);
    queryRegionRequest(parentId)
      .then((res) => {
        if (res.data.Error) {
          alert(res.data.Error.Message);
        } else {
          cb && cb(res.data.Data || []);
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    /*queryRegion(null, (data: any) => {
      setTreeData(u.map(data, (x) => buildNode(x)));
    });*/
    setTreeData([{ key: '-1', title: '中国大陆' }]);
  }, []);

  const onLoadData = (node: any) => {
    const { key, children } = node;
    console.log(node);

    var handledNodesKey: Array<string> = [];
    const setChildren = (treeNodes: Array<any>, childrenData: Array<any>) => {
      if (u.isEmpty(treeNodes)) {
        return;
      }
      for (var i = 0; i < treeNodes.length; i++) {
        var node = treeNodes[i];
        if (handledNodesKey.indexOf(node.key) >= 0) {
          return;
        }
        handledNodesKey.push(node.key);
        if (node.key === key) {
          node.children = childrenData;
          break;
        } else {
          setChildren(node.children, childrenData);
        }
      }
    };

    return new Promise<void>((resolve, reject) => {
      if (children) {
        resolve();
      } else {
        _loading(true);
        queryRegionRequest(key)
          .then((res) => {
            if (res.data.Error) {
              alert(res.data.Error.Message);
            } else {
              var responseData = res.data.Data || [];

              var treeDataCopy = [...treeData];
              setChildren(treeDataCopy, u.map(responseData, buildNode));
              console.log(treeDataCopy);
              setTreeData(treeDataCopy);

              if (u.isEmpty(responseData)) {
                message.info('暂无数据');
              }
            }
            resolve();
          })
          .catch((e) => {
            reject(e);
          })
          .finally(() => {
            _loading(false);
          });
      }
    });
  };

  return (
    <>
      <Card
        size="small"
        title="省市区"
        extra={null}
        style={{
          minHeight: 300,
        }}
      >
        <Tree loadData={onLoadData} treeData={treeData} blockNode />
      </Card>
    </>
  );
};

export default App;
