// TODO: Remove and consume from package. I'll make sure this happens before we release.
declare module '@impartner/api' {
  export namespace prm {
    export namespace asset {
      interface IUrls {
        contentType?: string;
        download: string;
        embed: string;
        pdf?: string;
        thumbnails: IThumbnails;
        isEmbeddable: boolean;
      }

      interface IThumbnails {
        '320x200': string;
        '640x400': string;
        '800x600': string;
        source: string;
      }

      interface IAssetShareStatusResult {
        id: number;
        isShareable: boolean;
        alreadyShared: boolean;
        shareurl: string;
      }

      interface IAsset {
        id: number;
        name: string;
        category?: IAssetCategory;
        'language.name'?: string;
        'category.name'?: string;
        'category.color'?: string;
        filename?: string;
        code?: string;
        subject?: string;
        type?: string;
        totalLikes?: number;
        activeStartDate?: string;
        viewCount?: string;
        thumbnail?: string;
        downloadCount?: number;
        shortDescription?: string;
        textBody?: string;
        createdBy?: number;
        contentType?: string;
        extension?: string;
        link?: string;
        sharedCount?: string;
        sharedHitCount?: string;
        version?: string;
        hasUserLiked: boolean;
        'cobrandingTemplate.isPublished'?: string;
      }

      interface IPlaybookAssets<T> {
        id: number;
        assets: T[];
      }
    }

    interface IAssetCategory {
      id: number;
      name: string;
      color: string;
      categoryIndex?: number;
    }

    interface IAssetNewCategory {
      name: string;
      color: string;
    }

    interface IAssetPlaybook {
      id: number;
      name: string;
      assetIds?: number[];
    }

    interface IAssetCollection {
      id: number;
      name: string;
      hasChildren: boolean;
      parentId: number | null;
      displayOrder: number;
    }

    interface ICobrandedDocument {
      id: number;
      name: string;
      created?: Date;
      'pdf.id'?: number;
      'pdf.fileHandle.getDownloadUrl(\'00:30:00\', \'Inline\') url'?: string;
    }

    interface IAssetCollectionUpdateName {
      id: number;
      name: string;
    }
  }
}
